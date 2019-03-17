using System;
using System.Runtime.Serialization;

namespace PhilipsSignageDisplaySicp
{
    [Serializable]
    internal class SicpNotAcknowledgedException : Exception
    {
        public SicpNotAcknowledgedException(byte[] commandData) : base("Command was not acknowledged (NACK) by the reciever. Command sent: " + BitConverter.ToString(commandData)) { }
        public SicpNotAcknowledgedException(string message) : base(message) { }
        public SicpNotAcknowledgedException(string message, Exception innerException) : base(message, innerException) { }
        protected SicpNotAcknowledgedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}