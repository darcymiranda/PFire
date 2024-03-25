using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.Messages.Bidirectional;

namespace PFire.Core.Protocol.XFireAttributes
{
    public class MessageAttribute : XFireAttribute
    {
        private static readonly Dictionary<uint, IMessage> MESSAGE_TYPES = new Dictionary<uint, IMessage>
        {
            { 0, new ChatMessage() },
            { 1, new ChatAcknowledgement() }
        };

        private IMessage CreateMessage(uint type)
        {
            if(!MESSAGE_TYPES.ContainsKey(type))
            {
                throw new UnknownMessageTypeException((XFireMessageType)type);
            }

            return (IMessage)Activator.CreateInstance(MESSAGE_TYPES[type].GetType());
        }

        public override Type AttributeType => typeof(Dictionary<string, IMessage>);

        public override byte AttributeTypeId => 0x15;

        public override dynamic ReadValue(BinaryReader reader)
        {
            var values = new Dictionary<string, IMessage>();
            var mapLength = reader.ReadByte();

            for(var i = 0; i < mapLength; i++)
            {
                var messageTypeName = ReadInt8String(reader);
                var messageType = XFireAttributeFactory.Instance.GetAttribute(reader.ReadByte()).ReadValue(reader);
                var message = MessageSerializer.Deserialize(reader, CreateMessage(messageType));
                values.Add(messageTypeName, message);
            }

            return values;
        }

        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
        }

        private string ReadInt8String(BinaryReader reader)
        {
            var length = reader.ReadByte();
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }
    }
}
