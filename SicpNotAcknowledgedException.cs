using System;
using System.Runtime.Serialization;

namespace PhilipsSignageDisplaySicp
{
    [Serializable]
    internal class SicpNotAcknowledgedException : Exception
    {
        public SicpNotAcknowledgedException() : base("Command was not acknowledged (NACK) by the reciever.") { }
        public SicpNotAcknowledgedException(string message) : base(message) { }
        public SicpNotAcknowledgedException(string message, Exception innerException) : base(message, innerException) { }
        protected SicpNotAcknowledgedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}