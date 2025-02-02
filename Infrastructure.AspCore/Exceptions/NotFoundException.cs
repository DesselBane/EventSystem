﻿using System;
using System.Net;

namespace Infrastructure.AspCore.Exceptions
{
    public class NotFoundException : InvalidRestOperationException
    {
        #region Properties

        public override int ResponseCode => (int) HttpStatusCode.NotFound;

        #endregion

        #region Constructors

        public NotFoundException(int id, string typeName, Guid customError, string additionalMessage = "", Exception innerException = null)
            : this(id.ToString(), typeName, customError, additionalMessage, innerException)
        {
        }

        public NotFoundException(string id, string typeName, Guid customError, string additionalMessage = "", Exception innerException = null)
            : base($"No {typeName} with Id: {id} was found." + additionalMessage, customError, innerException)
        {
        }

        #endregion
    }
}