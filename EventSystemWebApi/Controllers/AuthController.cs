using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataContracts;
using Infrastructure.ErrorCodes;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EventSystemWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AuthController : Controller
    {
        #region Vars

        private readonly IAuthenticationService _authService;

        #endregion

        #region Constructors

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        #endregion

        [AllowAnonymous]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TokenDTO), Description = "Returns serialized JWT Token")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.INVALID_USERNAME_OR_PASSWORD + "\n\nInvalid Username or Password")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + AuthenticationErrorCodes.USERACCOUNT_NOT_ACTIVATED + "\n\nUseraccount not activated")]
        public virtual Task<TokenDTO> Login([FromBody] LoginDTO loginDto)
        {
            return _authService.AuthenticateAsync(loginDto.Username, loginDto.Password);
        }

        [AllowAnonymous]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TokenDTO))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(TokenDTO), Description = AuthenticationErrorCodes.USER_NOT_FOUND + "\n\nNo User matching this access token user email claim was found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.REFRESH_TOKENS_DIDNT_MATCH + "\n\n Refresh Tokens didnt match")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(ExceptionDTO), Description = "\n" + AuthenticationErrorCodes.REFRESH_TOKEN_EXPIRED + "\n\n Refresh Token expired")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(ExceptionDTO), Description = "\n" + AuthenticationErrorCodes.ACCESS_TOKEN_INVALID + "\n\n Access Token is invalid")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<TokenDTO> RefreshToken([FromBody] TokenDTO token)
        {
            return _authService.AuthenticateTokenAsync(token);
        }

        [AllowAnonymous]
        [HttpPut]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.USERNAME_NO_EMAIL + "\n\nOccures if the Username is not an EMail")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.USERNAME_EXISTS_ALREADY + "\n\nOccurs if the username already exists")]
        public virtual async Task Register(string username)
        {
            await _authService.RegisterUserAsync(username).ConfigureAwait(false);
        }

        [AllowAnonymous]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.RESET_HAST_DOESNT_EXIST + "\n\nResetHash doesnt Exist")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual async Task ChangePassword([FromBody] PasswordUpdateDTO password)
        {
            await _authService.ChangePasswordAsync(password.Password, password.ResetHash);
        }

        [AllowAnonymous]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.USER_NOT_FOUND + "\n\nUsername doesnt Exist")]
        public virtual async Task ResetPassword(string username)
        {
            await _authService.ResetPasswordAsync(username);
        }

        [HttpPost]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.USERNAME_NO_EMAIL + "\n\nUsername has no EMail Format")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.USERNAME_EXISTS_ALREADY + "\n\nUsername already exists")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.USER_NOT_FOUND + "\n\n Occures if a valid AccessToken is provided which points to a non existing user")]
        public virtual Task<TokenDTO> ChangeUsername(string username)
        {
            return _authService.ChangeUsernameAsync(username, HttpContext.User.Identity as ClaimsIdentity);
        }

        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = AuthenticationErrorCodes.USER_NOT_FOUND + "\n\nUser doesnt exist anymore")]
        public virtual async Task DeleteAccount()
        {
            await _authService.DeleteAccountAsync(HttpContext.User.Identity as ClaimsIdentity);
        }
    }
}