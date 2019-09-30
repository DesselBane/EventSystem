using System;
using System.Runtime.Serialization;

namespace Infrastructure.Exceptions
{
    public class MissingConfigurationException : Exception
    {
        public MissingConfigurationException()
        {
        }

        protected MissingConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MissingConfigurationException(string message) : base(message)
        {
        }

        public MissingConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}