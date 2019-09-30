using System;
using Infrastructure.Email;
using Newtonsoft.Json;

namespace EventSystemWebApi.Options
{
    public sealed class EmailServiceConfiguration : IEmailServiceConfiguration
    {
        #region Properties

        /// <summary>
        ///     The Port of the Email Server
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     The Hostname of the EMail Server
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///     Determins if any authentication for the SMTP Server is required
        /// </summary>
        public bool UseAuthentication { get; set; }

        /// <summary>
        ///     The username for the SMTP Auth
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     The Password for the SMTP Auth
        /// </summary>
        public string Password { get; set; }

        public bool UseDummyService { get; set; }
        
        #endregion

        public void ThrowIfMisconfigured()
        {
            if(UseDummyService)
                return;
            
            if (Port <= 0)
                throw new ArgumentException("port must be a value of greater then 0", nameof(Port));
            if (string.IsNullOrWhiteSpace(Host))
                throw new ArgumentException("host cannot be null or whitespace", nameof(Host));

            if (UseAuthentication)
            {
                if (string.IsNullOrWhiteSpace(Username))
                    throw new ArgumentException("username cannot be null or whitespace", nameof(Username));
                if (string.IsNullOrWhiteSpace(Password))
                    throw new ArgumentException("password cannot be null or whitespace", nameof(Password));
            }

        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}