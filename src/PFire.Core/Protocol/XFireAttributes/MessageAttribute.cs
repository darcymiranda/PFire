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
        private readonly static Dictionary<int, IMessage> MESSAGE_TYPES = new Dictionary<int, IMessage>()
        {
            { 0, new ChatMessage() },
            { 1, new ChatAcknowledgement() }
        };

        private IMessage CreateMessage(short type)
        {
            if (!MESSAGE_TYPES.ContainsKey(type))
            {
                throw new UnknownMessageTypeException((XFireMessageType)type);
            }
            return (IMessage)Activator.CreateInstance(MESSAGE_TYPES[type].GetType());
        }

        public override Type AttributeType
        {
            get { return typeof(Dictionary<string, IMessage>); }
        }

        public override byte AttributeTypeId
        {
            get { return 0x15; }
        }

        public override dynamic ReadValue(BinaryReader reader)
        {
            var values = new Dictionary<string, IMessage>();
            var mapLength = reader.ReadByte();

            for (int i = 0; i < mapLength; i++)
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

        private void WriteInt8String(BinaryWriter writer, string value)
        {
            writer.Write((byte)value.Length);
            writer.Write(Encoding.UTF8.GetBytes(value));
        }

        private string ReadInt8String(BinaryReader reader)
        {
            var length = reader.ReadByte();
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }
    }
}
