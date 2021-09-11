using System;
using System.Runtime.Serialization;

namespace Infra.Email.Smtp.Exceptions
{
    [Serializable]
    public class CastSmtpParamFailException : Exception
    {
        public CastSmtpParamFailException(string message)
            : base(message) { }

        public CastSmtpParamFailException(string message, Exception innerException)
            : base(message, innerException) { }

        protected CastSmtpParamFailException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
