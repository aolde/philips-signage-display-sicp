using System;
using System.Runtime.Serialization;

namespace PhilipsSignageDisplaySicp
{
    [Serializable]
    internal class SicpNotAvailableException : Exception
    {
        public SicpNotAvailableException(byte[] commandData) : base("Command is valid but not supported in the current implementation (Not Available/NAV). Command sent: " + BitConverter.ToString(commandData)) { }
        public SicpNotAvailableException(string message) : base(message) { }
        public SicpNotAvailableException(string message, Exception innerException) : base(message, innerException) { }
        protected SicpNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}