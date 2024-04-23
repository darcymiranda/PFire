using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.Messages.Inbound;
using PFire.Core.Util;

namespace PFire.Core.Protocol
{
    internal static class MessageSerializer
    {
        private static readonly int MESSAGE_SIZE_LENGTH_IN_BYTES = 2;

        public static IMessage Deserialize(byte[] data)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            var messageTypeId = reader.ReadInt16();
            var xMessageType = (XFireMessageType)messageTypeId;

            var messageType = XFireMessageTypeFactory.Instance.GetMessageType(xMessageType);
            var message = Activator.CreateInstance(messageType) as IMessage;
            return Deserialize(reader, message);
        }

        public static IMessage Deserialize(BinaryReader reader, IMessage messageBase)
        {
            var messageType = messageBase.GetType();
            var fieldInfo = messageType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            var attributeCount = reader.ReadByte();

            for (var i = 0; i < attributeCount; i++)
            {
                var attributeName = GetAttributeName(reader, messageType);

                var attributeType = reader.ReadByte();

                var value = XFireAttributeFactory.Instance.GetAttribute(attributeType).ReadValue(reader);

                var field = fieldInfo
                    .Where(a => a.GetCustomAttribute<XMessageField>() != null)
                    .FirstOrDefault(a =>
                    {
                        var attribute = a.GetCustomAttribute<XMessageField>();
                        if (attribute != null)
                        {
                            if (attributeName.Length == 1)
                            {
                                // If attributeName is a single byte, compare with the attribute's NameAsBytes
                                return attribute.NameAsBytes.SequenceEqual(attributeName);
                            }
                            else
                            {
                                // If attributeName is a byte array with length > 1, compare with the attribute's Name converted to string
                                return Encoding.UTF8.GetString(attributeName) == attribute.Name;
                            }
                        }
                        return false;
                    });

                if (field != null)
                {
                    field.SetValue(messageBase, value);
                }
                else
                {
                    Debug.WriteLine($"WARN: No attribute defined for {attributeName} on class {messageType.Name}");
                }
            }

            Debug.WriteLine($"Deserialized [{messageType}]: {messageBase}");

            return messageBase;
        }

        private static byte[] GetAttributeName(BinaryReader reader, Type messageType)
        {
            HashSet<Type> messageTypeSet = new HashSet<Type>
            {
                typeof(StatusChange),
                typeof(GameServerFetchAll),
                typeof(GroupCreate),
                typeof(GroupMemberAdd),
                typeof(GroupMemberRemove),
                typeof(GroupRemove),
                typeof(GroupRename),
                typeof(GameClientData)
            };

            byte count = messageTypeSet.Contains(messageType) ? (byte)1 : reader.ReadByte();

            // Check if count is 1, indicating a single byte
            if (count == 1)
            {
                // Read the single byte and treat it as a numeric value
                byte numericValue = reader.ReadByte();
                return [numericValue];
            }
            else
            {
            // Read the bytes for the attribute name
            var readBytes = reader.ReadBytes(count);
                return readBytes;
            }
        }

        public static byte[] Serialize(IMessage message)
        {
            var payload = WritePayloadFromMessage(message);
            var payloadShort = (short)(payload.Length + MESSAGE_SIZE_LENGTH_IN_BYTES);
            var payloadLength = BitConverter.GetBytes(payloadShort);

            var finalPayload = ByteHelper.CombineByteArray(payloadLength, payload);

            Debug.WriteLine($"Serialized [{message}]: {BitConverter.ToString(finalPayload)}");

            return finalPayload;
        }

        private static byte[] WritePayloadFromMessage(IMessage message)
        {
            var propertyInfo = message.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var attributesToBeWritten = new List<Tuple<XMessageField, byte, dynamic>>();
            propertyInfo.Where(a => Attribute.IsDefined(a, typeof(XMessageField)))
                        .ToList()
                        .ForEach(property =>
                        {
                            var propertyValue = property.GetValue(message);
                            var attributeDefinition = property.GetCustomAttribute<XMessageField>();
                            var attribute = XFireAttributeFactory.Instance.GetAttribute(property.PropertyType);

                            attributesToBeWritten.Add(
                                Tuple.Create<XMessageField, byte, dynamic>(
                                    attributeDefinition,
                                    attribute.AttributeTypeId,
                                    propertyValue
                                )
                            );
                        });

            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            writer.Write((short)message.MessageTypeId);
            writer.Write((byte)attributesToBeWritten.Count);
            attributesToBeWritten.ForEach(a =>
            {
                var attribute = XFireAttributeFactory.Instance.GetAttribute(a.Item2);
                if (a.Item1.NonTextualName)
                {
                    attribute.WriteNameWithoutLengthPrefix(writer, a.Item1.NameAsBytes);
                    attribute.WriteType(writer);
                    attribute.WriteValue(writer, a.Item3);
                }
                else
                {
                    attribute.WriteAll(writer, a.Item1.Name, a.Item3);
                }
            });

            return ms.ToArray();
        }
    }
}
