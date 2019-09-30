using System;
using System.Net;

namespace Infrastructure.AspCore.Exceptions
{
    public class ForbiddenException : InvalidRestOperationException
    {
        #region Properties

        public override int ResponseCode => (int) HttpStatusCode.Forbidden;

        #endregion

        #region Constructors

        public ForbiddenException(string message, Guid customErrorCode, Exception innerException = null)
            : base(message, customErrorCode, innerException)
        {
        }

        #endregion
    }
}