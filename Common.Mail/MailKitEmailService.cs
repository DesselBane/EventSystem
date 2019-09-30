using System;
using System.Threading.Tasks;
using Common.Mail.Extensions;
using Infrastructure.Email;
using MailKit.Net.Smtp;

namespace Common.Mail
{
    public class MailKitEmailService : IEmailService
    {
        #region Vars

        private readonly object _configSyncHandle = new object();

        #endregion

        #region Properties

        public IEmailServiceConfiguration Configuration { get; private set; }

        #endregion

        #region Constructors

        public MailKitEmailService(IEmailServiceConfiguration config)
        {
            Configuration = config;
        }

        #endregion

        public Task SendMessageAsync(IEmail mail)
        {
            return Task.Run(() =>
            {
                using (var client = new SmtpClient())
                {
                    lock (_configSyncHandle)
                    {
                        Console.WriteLine(Configuration.ToString());
                        Console.WriteLine(mail.ToString());
                        mail.From = Configuration.Username;
                        
                        // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        client.Connect(Configuration.Host, Configuration.Port,true);

                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        client.AuthenticationMechanisms.Remove("XOAUTH2");

                        // Note: only needed if the SMTP server requires authentication
                        if (Configuration.UseAuthentication)
                            client.Authenticate(Configuration.Username, Configuration.Password);

                        client.Send(mail.ToMimeMessage());
                        client.Disconnect(true);
                    }
                }
            });
        }

        
    }
}