using System;
using System.Threading.Tasks;
using Infrastructure.Email;

namespace Common.Mail
{
    public class DummyMailService : IEmailService
    {
        #region Properties

        public IEmailServiceConfiguration Configuration { get; }

        #endregion

        #region Constructors

        public DummyMailService(IEmailServiceConfiguration config)
        {
            Configuration = config;
        }

        #endregion

        public Task SendMessageAsync(IEmail mail)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("--------------------------- EMAIL ---------------------------");
                Console.WriteLine($"Message to: {mail.To}");
                Console.WriteLine($"Message from: {mail.From}");
                Console.WriteLine($"Subject: {mail.Subject}");
                Console.WriteLine($"Message: {mail.Body}");
            });
        }

     
    }
}