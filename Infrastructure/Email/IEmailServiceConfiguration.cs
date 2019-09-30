namespace Infrastructure.Email
{
    public interface IEmailServiceConfiguration
    {
        /// <summary>
        ///     The Port of the Email Server
        /// </summary>
        int Port { get; }

        /// <summary>
        ///     The Hostname of the EMail Server
        /// </summary>
        string Host { get; }

        /// <summary>
        ///     Determins if any authentication for the SMTP Server is required
        /// </summary>
        bool UseAuthentication { get; }

        /// <summary>
        ///     The username for the SMTP Auth
        /// </summary>
        string Username { get; }

        /// <summary>
        ///     The Password for the SMTP Auth
        /// </summary>
        string Password { get; }

        bool UseDummyService { get; }
        void ThrowIfMisconfigured();
    }
}