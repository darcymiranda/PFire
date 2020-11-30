using PFire.Core.Protocol.Messages;
using System;

namespace PFire.Protocol
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
