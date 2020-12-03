using System;

namespace PFire.Core.Protocol.Messages
{
    public sealed class UnknownMessageTypeException : Exception
    {
        public UnknownMessageTypeException(XFireMessageType messageType) : base($"Unknown message type: {messageType}") {}
    }
}
