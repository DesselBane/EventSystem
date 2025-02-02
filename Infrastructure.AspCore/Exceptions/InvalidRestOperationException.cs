﻿using System;

namespace Infrastructure.AspCore.Exceptions
{
    public abstract class InvalidRestOperationException : InvalidOperationException
    {
        #region Properties

        public abstract int ResponseCode { get; }
        public Guid CustomErrorCode { get; }

        #endregion

        #region Constructors

        protected InvalidRestOperationException(string message, Guid customErrorCode, Exception innerException = null)
            : base(message, innerException)
        {
            CustomErrorCode = customErrorCode;
        }

        #endregion
    }
}