using System.Threading.Tasks;

namespace Infrastructure.Email
{
    public interface IEmailService
    {
        #region Properties

        /// <summary>
        ///     Gets the EmailServiceConfiguration object currently used
        /// </summary>
        IEmailServiceConfiguration Configuration { get; }

        #endregion

        /// <summary>
        ///     Sends an Email
        /// </summary>
        /// <param name="mail">The Email to be sent</param>
        /// <returns></returns>
        Task SendMessageAsync(IEmail mail);

    }
}