using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Common.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Security;
using Infrastructure.ErrorCodes;

namespace Common.Security
{
    public class AuthenticationValidator
    {
        #region Vars

        private readonly DataContext _securityContext;

        #endregion

        #region Constructors

        public AuthenticationValidator(DataContext securityContext)
        {
            _securityContext = securityContext;
        }

        #endregion

        public void ValidateUsername(string username)
        {
            if (!username.IsValidEmail())
                throw new UnprocessableEntityException("Username has to be an EMail", Guid.Parse(AuthenticationErrorCodes.USERNAME_NO_EMAIL));
        }

        public void ValidateUserExists(string username)
        {
            if (!_securityContext.Users.Any(x => string.Equals(x.EMail, username, StringComparison.CurrentCultureIgnoreCase)))
                throw new NotFoundException(username, nameof(User), Guid.Parse(AuthenticationErrorCodes.USER_NOT_FOUND));
        }

        public void ValidateUsernameDoesntExist(string username)
        {
            if (_securityContext.Users.Any(x => string.Equals(x.EMail, username, StringComparison.CurrentCultureIgnoreCase)))
                throw new ConflictException("Username already exists", Guid.Parse(AuthenticationErrorCodes.USERNAME_EXISTS_ALREADY));
        }

        public void ValidateResetHashExists(string resetHash)
        {
            if (!_securityContext.Users.Any(x => x.ResetHash == resetHash))
                throw new NotFoundException("", "ResetHash doesnt Exist", Guid.Parse(AuthenticationErrorCodes.RESET_HAST_DOESNT_EXIST));
        }

        public void ValidateJwtToken(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            if (!jwtHandler.CanReadToken(token))
                throw new UnauthorizedException("Invalid Access Token", Guid.Parse(AuthenticationErrorCodes.ACCESS_TOKEN_INVALID));

            var jwtToken = jwtHandler.ReadJwtToken(token);

            if (!_securityContext.Users.Any(x => string.Equals(x.EMail, jwtToken.Subject, StringComparison.CurrentCultureIgnoreCase)))
                throw new NotFoundException(jwtToken.Subject, nameof(User), Guid.Parse(AuthenticationErrorCodes.USER_NOT_FOUND));
        }
    }
}