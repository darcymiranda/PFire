using System;
using PFire.Core.Session;
using PFire.Core.Util;

namespace PFire.Core.Protocol.Messages
{
    internal abstract class XFireMessage : IMessage
    {
        protected XFireMessage(XFireMessageType typeId)
        {
            MessageTypeId = typeId;
        }

        public XFireMessageType MessageTypeId { get; }

        public virtual void Process(XFireClient client)
        {
            // base implementation is to do nothing
            ConsoleLogger.Log($" *** Unimplemented processing for message type {MessageTypeId}", ConsoleColor.Magenta);
        }
    }
}
