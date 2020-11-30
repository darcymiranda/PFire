using PFire.Protocol.Messages;
using PFire.Session;
using System;

namespace PFire.Core.Protocol.Messages
{
    public abstract class XFireMessage : IMessage
    {
        protected XFireMessageType MessageType;

        public XFireMessageType MessageTypeId { get; protected set; }

        protected XFireMessage(XFireMessageType typeId)
        {
            MessageTypeId = typeId;
        }

        public virtual void Process(XFireClient client)
        {
            // base implmenetation is to do nothing
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($" *** Unimplemented processing for message type {MessageTypeId}");
            Console.ForegroundColor = oldColor;
        }
    }
}
