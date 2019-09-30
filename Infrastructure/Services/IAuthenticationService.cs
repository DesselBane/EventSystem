using System.Security.Claims;
using System.Threading.Tasks;
using Infrastructure.DataContracts;
using Infrastructure.DataModel.Security;

namespace Infrastructure.Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        ///     Performs the Authentication/Authorization. Looks up the username and creates a ClaimsIdentity
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Returns a new ClaimsIdentity with all claims that are available</returns>
        Task<TokenDTO> AuthenticateAsync(string username, string password);

        Task<TokenDTO> AuthenticateTokenAsync(TokenDTO token);

        /// <summary>
        ///     Creates an Useraccount for the given username and password.
        /// </summary>
        /// <param name="username">The username to be used</param>
        /// <returns>Returns a new ClaimsIdentity for the Useraccount</returns>
        Task RegisterUserAsync(string username);

        /// <summary>
        ///     Sets the Password after a reset was initiated
        /// </summary>
        /// <param name="newPassword">The new Password to be set</param>
        /// <param name="resetHash">The reset Hash Identification</param>
        /// <returns></returns>
        Task ChangePasswordAsync(string newPassword, string resetHash);

        /// <summary>
        ///     Initiates the Reset Password Process
        /// </summary>
        /// <param name="username">The username to be resetted</param>
        /// <returns></returns>
        Task ResetPasswordAsync(string username);

        /// <summary>
        ///     Updates the Username
        /// </summary>
        /// <param name="username">The new Username</param>
        /// <param name="ident"></param>
        /// <returns>Returns the new JWT Token</returns>
        Task<TokenDTO> ChangeUsernameAsync(string username, ClaimsIdentity ident);

        /// <summary>
        ///     Deletes the specified Identity
        /// </summary>
        /// <param name="ident">Identity to be deleted</param>
        Task DeleteAccountAsync(ClaimsIdentity ident);

        Task<User> GetCurrentUserAsync();
    }
}