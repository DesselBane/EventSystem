namespace Infrastructure.Email
{
    public interface IEmail
    {
        #region Properties

        /// <summary>
        ///     The Recipients Email Address
        /// </summary>
        string To { get; }

        /// <summary>
        ///     The Email Address the Email will be sent from
        /// </summary>
        string From { get; set; }

        /// <summary>
        ///     The Subject duh
        /// </summary>
        string Subject { get; }

        /// <summary>
        ///     What you want to say
        /// </summary>
        string Body { get; }

        #endregion
    }
}