using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataContracts;
using Infrastructure.DataModel;
using Infrastructure.DataModel.People;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.Email;
using Infrastructure.ErrorCodes;
using Infrastructure.Options;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Common.Security
{
    public class JwtAuthenticationService : IAuthenticationService
    {
        #region Vars

        private readonly ClaimsIdentity _identity;
        private readonly IHostingOptions _hostingOptions;
        private readonly IEmailService _mailService;

        private readonly JwtTokenOptions _options;
        private readonly DataContext _securityContext;

        #endregion

        #region Properties

        private DbSet<User> UserRepository => _securityContext.Users;

        #endregion

        #region Constructors

        public JwtAuthenticationService(JwtTokenOptions options,
                                        DataContext securityContext,
                                        IEmailService mailService,
                                        ClaimsIdentity identity,
                                        IHostingOptions hostingOptions)
        {
            _options = options;
            _securityContext = securityContext;
            _mailService = mailService;
            _identity = identity;
            _hostingOptions = hostingOptions;

            options.ThrowIfInvalidOptions();
        }

        #endregion


        public async Task<TokenDTO> AuthenticateAsync(string username, string password)
        {
            var identity = await AuthenticateUserAsync(username, password);

            if (!identity.IsAuthenticated)
                return new TokenDTO();

            return await GenerateTokenAsync(identity);
        }

        public async Task<TokenDTO> AuthenticateTokenAsync(TokenDTO token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtHandler.ReadJwtToken(token.AccessToken);

            var user = await _securityContext.Users
                                             .Include(x => x.Claims)
                                             .FirstOrDefaultAsync(x => x.EMail == jwtToken.Subject);

            if (user.RefreshToken != token.RefreshToken)
                throw new UnauthorizedException("Refresh Tokens dont match", Guid.Parse(AuthenticationErrorCodes.REFRESH_TOKENS_DIDNT_MATCH));

            if (!user.RefreshTokenExpiration.HasValue || user.RefreshTokenExpiration.Value < DateTime.UtcNow)
                throw new UnauthorizedException("Refresh Token Expired", Guid.Parse(AuthenticationErrorCodes.REFRESH_TOKEN_EXPIRED));

            return await GenerateTokenAsync(user);
        }

        public async Task RegisterUserAsync(string username)
        {
            var user = CreateNewUser(username);
            UserRepository.Add(user);

            user.Person = new RealPerson();
            await _securityContext.SaveChangesAsync().ConfigureAwait(false);

            await ResetPasswordAsync(username);
        }

        public async Task ChangePasswordAsync(string newPassword, string resetHash)
        {
            var user = await UserRepository.FirstOrDefaultAsync(x => x.ResetHash == resetHash).ConfigureAwait(false);

            UpdatePassword(user, newPassword);
            await _securityContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task ResetPasswordAsync(string username)
        {
            var user = await _securityContext.Users
                                             .FirstOrDefaultAsync(x => string.Equals(x.EMail, username, StringComparison.CurrentCultureIgnoreCase))
                                             .ConfigureAwait(false);

            user.ResetHash = GenerateResetHash();
            await _securityContext.SaveChangesAsync();

            await _mailService.SendMessageAsync(new SimpleEmail
            {
                To = user.EMail,
                From = "No-reply@EventSystemWebmaster.de",
                Subject = "You requested a password reset.",
                Body = $"Please click this link to set your new Password: {_hostingOptions.Url}/auth/changePassword?user={user.ResetHash}"
            });
        }

        public async Task<TokenDTO> ChangeUsernameAsync(string username, ClaimsIdentity ident)
        {
            var user = await UserRepository.Include(x => x.Claims)
                                           .FirstOrDefaultAsync(x => string.Equals(x.EMail, ident.GetUsername(), StringComparison.CurrentCultureIgnoreCase));

            UpdateUsername(user, username);
            await _securityContext.SaveChangesAsync();
            return await GenerateTokenAsync(user);
        }

        public async Task DeleteAccountAsync(ClaimsIdentity ident)
        {
            var user = await UserRepository.FirstOrDefaultAsync(x => string.Equals(x.EMail, ident.GetUsername(), StringComparison.CurrentCultureIgnoreCase));

            UserRepository.Remove(user);
            await _securityContext.SaveChangesAsync();
        }

        public Task<User> GetCurrentUserAsync()
        {
            return _securityContext.Users.FirstOrDefaultAsync(x => string.Equals(x.EMail, _identity.GetUsername(), StringComparison.CurrentCultureIgnoreCase));
        }

        #region Impl

        public static void UpdatePassword(User user, string newPassword)
        {
            var pwSalt = newPassword.HashPassword();
            user.Password = pwSalt.Item1;
            user.Salt = pwSalt.Item2;
            user.ResetHash = null;
        }

        public static User CreateNewUser(string username)
        {
            var user = new User
            {
                EMail = username,
                Password = null
            };

            user.Claims.Add(new UsernameClaim(username));

            return user;
        }

        public static void UpdateUsername(User user, string newUsername)
        {
            user.Claims.RemoveAll(x => x.Type == UsernameClaim.USERNAME_CLAIM_TYPE);

            user.EMail = newUsername;
            user.Claims.Add(new UsernameClaim(newUsername));
        }

        private static string GenerateResetHash()
        {
            return Guid.NewGuid().ToString().HashPassword().Item1;
        }

        private async Task<TokenDTO> GenerateTokenAsync(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, identity.Name),
                new Claim(JwtRegisteredClaimNames.Jti, await _options.NonceGenerator().ConfigureAwait(false)),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixEpochDate().ToString(), ClaimValueTypes.Integer64)
            };

            claims.AddRange(identity.Claims);

            var token = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                claims.ToArray(),
                now,
                now.Add(_options.Expiration),
                _options.SigningCredentials);

            var refreshToken = Guid.NewGuid().ToString();

            var user = await _securityContext.Users.FirstOrDefaultAsync(x => x.EMail == identity.Name);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = now.Add(_options.RefreshTokenExpiration);
            await _securityContext.SaveChangesAsync();


            return new TokenDTO
            {
                RefreshToken = user.RefreshToken,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessToken_ValidUntil = now.Add(_options.Expiration),
                RefreshToken_ValidUntil = user.RefreshTokenExpiration.Value
            };
        }

        private async Task<ClaimsIdentity> AuthenticateUserAsync(string username, string password)
        {
            var user = await UserRepository.Include(x => x.Claims)
                                           .FirstOrDefaultAsync(x => string.Equals(x.EMail, username, StringComparison.CurrentCultureIgnoreCase))
                                           .ConfigureAwait(false);

            ValidateUser(user, password);

            return user;
        }

        public static void ValidateUser(User user, TokenDTO token)
        {
            if (user == null)
                throw new NotFoundException("", nameof(User), Guid.Parse(AuthenticationErrorCodes.USER_NOT_FOUND));

            if (user.RefreshToken != token.RefreshToken)
                throw new UnauthorizedException("Refresh Tokens dont match", Guid.Parse(AuthenticationErrorCodes.REFRESH_TOKENS_DIDNT_MATCH));
        }

        public static void ValidateUser(User user, string password)
        {
            if (user == null)
                throw new UnauthorizedException("Invalid Username", Guid.Parse(AuthenticationErrorCodes.INVALID_USERNAME_OR_PASSWORD));

            if (string.IsNullOrWhiteSpace(user.Password))
                throw new UnprocessableEntityException("Useraccount not activated", Guid.Parse(AuthenticationErrorCodes.USERACCOUNT_NOT_ACTIVATED));

            if (!password.ValidatePassword(user.Password, user.Salt))
                throw new UnauthorizedException("Invalid Password", Guid.Parse(AuthenticationErrorCodes.INVALID_USERNAME_OR_PASSWORD));
        }

        #endregion Impl
    }
}