using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Security;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataContracts;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.Authentication
{
    public class AuthControllerLoginTests : TestBase
    {
        [Fact]
        public async Task Login_Unauthorized_1_InvalidPassword()
        {
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var response = await _Client.PostAsync("/api/auth/login",
                new LoginDTO {Username = user.EMail, Password = "ajklfghnakjbgn"}.ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await response.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.INVALID_USERNAME_OR_PASSWORD), error.ErrorCode);
        }

        [Fact]
        public async Task Login_Unauthorized_1_InvalidUsername()
        {
            var response = await _Client.PostAsync("/api/auth/login", new LoginDTO {Username = "thisIsNoUser@gmx.de", Password = "abc123"}.ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await response.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.INVALID_USERNAME_OR_PASSWORD), error.ErrorCode);
        }

        [Fact]
        public async Task Login_Success()
        {
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var loginDTO = new LoginDTO {Password = ALL_TIME_PASSWORD, Username = user.EMail};


            var response = await _Client.PostAsync("/api/auth/login", loginDTO.ToStringContent());
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Login_422_1_UseraccountNotActivated()
        {
            var user = CreateUser();
            user.Password = null;
            user.Salt = null;
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var response = await _Client.PostAsync("/api/auth/login", new LoginDTO {Username = user.EMail, Password = "asdlkfj"}.ToStringContent());
            Assert.Equal((HttpStatusCode) 422, response.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await response.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.USERACCOUNT_NOT_ACTIVATED), error.ErrorCode);
        }

        [Fact]
        public async Task Login_422_0_NoData()
        {
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync("api/auth/login", "".ToStringContent());

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task RefreshToken_NotFound_1()
        {
            var token = await CreateAccessTokenAsync(Guid.NewGuid().ToString());

            var r = await _Client.PostAsync($"api/auth/refreshToken", new TokenDTO
            {
                AccessToken = token
            }.ToStringContent());

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.USER_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task RefreshToken_Unauthorized_TokenDidntMatch()
        {
            var user = await SetupAuthenticationAsync();
            var token = await CreateAccessTokenAsync(user.EMail);

            var r = await _Client.PostAsync("api/auth/refreshtoken", new TokenDTO
            {
                AccessToken = token,
                RefreshToken = Guid.NewGuid().ToString()
            }.ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.REFRESH_TOKENS_DIDNT_MATCH), error.ErrorCode);
        }

        [Fact]
        public async Task RefreshToken_Unauthorized_RefreshTokenExpired()
        {
            var user = CreateUser();
            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(-1);

            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync("api/auth/refreshtoken", new TokenDTO
            {
                RefreshToken = user.RefreshToken,
                AccessToken = await CreateAccessTokenAsync(user.EMail)
            }.ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.REFRESH_TOKEN_EXPIRED), error.ErrorCode);
        }

        [Fact]
        public async Task RefreshToken_Unauthorized_AccessTokenIsInvalid()
        {
            var r = await _Client.PostAsync("api/auth/refreshToken", new TokenDTO
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString()
            }.ToStringContent());

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());

            Assert.Equal(Guid.Parse(AuthenticationErrorCodes.ACCESS_TOKEN_INVALID), error.ErrorCode);
        }

        [Fact]
        public async Task RefreshToken_Success()
        {
            var user = CreateUser();
            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpiration = DateTime.Now.AddDays(60);
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync("api/auth/refreshToken", new TokenDTO
            {
                AccessToken = await CreateAccessTokenAsync(user.EMail),
                RefreshToken = user.RefreshToken
            }.ToStringContent());

            r.EnsureSuccessStatusCode();

            var dbUser = await CreateDataContext().Users.FirstOrDefaultAsync(x => x.EMail == user.EMail);
            var result = JsonConvert.DeserializeObject<TokenDTO>(await r.Content.ReadAsStringAsync());

            Assert.NotNull(dbUser);
            Assert.NotNull(result);
            Assert.Equal(dbUser.RefreshToken, result.RefreshToken);

            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParams = _Server.Host.Services.GetService(typeof(TokenValidationParameters)) as TokenValidationParameters;
            var principal = handler.ValidateToken(result.AccessToken, tokenValidationParams, out var _);
            Assert.True(principal.Identity.IsAuthenticated);
        }

        private async Task<string> CreateAccessTokenAsync(string username)
        {
            var options = (JwtTokenOptions) _Server.Host.Services.GetService(typeof(JwtTokenOptions));

            var now = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, await options.NonceGenerator().ConfigureAwait(false)),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixEpochDate().ToString(), ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                options.Issuer,
                options.Audience,
                claims.ToArray(),
                now,
                now.Add(options.Expiration),
                options.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}