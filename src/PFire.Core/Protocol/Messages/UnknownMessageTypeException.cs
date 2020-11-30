using System;

namespace PFire.Core.Protocol.Messages
{
    public sealed class UnknownMessageTypeException : Exception
    {
        public UnknownMessageTypeException(XFireMessageType messageType) :
            base($"Unknown message type: {messageType}") 
        {
        }

        public UnknownMessageTypeException() : base()
        {
        }

        public UnknownMessageTypeException(string message) : base(message)
        {
        }

        public UnknownMessageTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
