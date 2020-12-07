using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.XFireAttributes;
using PFire.Protocol.Messages;
using PFire.Util;
using PFire.Protocol.Messages.Inbound;
using PFire.Core.Protocol.Messages;
using System.Collections;

namespace PFire.Protocol
{
    public static class MessageSerializer
    {
        private static readonly int MESSAGE_SIZE_LENGTH_IN_BYTES = 2;

        public static IMessage Deserialize(byte[] data)
        {
            using (var reader = new BinaryReader(new MemoryStream(data)))
            {
                short messageTypeId = reader.ReadInt16();
                var xMessageType = (XFireMessageType)messageTypeId;

                var messageType = MessageTypeFactory.Instance.GetMessageType(xMessageType);
                var message = Activator.CreateInstance(messageType) as IMessage;
                if (message.MessageTypeId == XFireMessageType.Unknown)
                {
                    ((UnknownMessageType)message).MessageCode = messageTypeId;
                }
                return Deserialize(reader, message);
            }
        }

        public static IMessage Deserialize(byte[] data, IMessage messageType)
        {
            using var reader = new BinaryReader(new MemoryStream(data));
            return Deserialize(reader, messageType);
        }

        public static IMessage Deserialize(BinaryReader reader, IMessage messageBase)
        {
            var messageType = messageBase.GetType();
            var fieldInfo = messageType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            var attributeCount = reader.ReadByte();

            for (var i = 0; i < attributeCount; i++)
            {
                // TODO: Be brave enough to find an elegant fix for this
                // XFire decides not to follow its own rules. Message type 32 does not have a prefix byte for the length of the attribute name
                // and breaks this code. Assume first byte after the attribute count as the attribute name
                string attributeName = null;
                if (messageType == typeof(StatusChange))
                {
                    attributeName = Encoding.UTF8.GetString(reader.ReadBytes(1));
                }
                else
                {
                    var attributeNameLength = reader.ReadByte();
                    attributeName = Encoding.UTF8.GetString(reader.ReadBytes(attributeNameLength));
                }
                var attributeType = reader.ReadByte();

                dynamic value = XFireAttributeFactory.Instance.GetAttribute(attributeType).ReadValue(reader);

                if (messageBase.MessageTypeId != XFireMessageType.Unknown)
                {
                    var field = fieldInfo.Where(a => a.GetCustomAttribute<XMessageField>() != null)
                        .FirstOrDefault(a => a.GetCustomAttribute<XMessageField>()?.Name == attributeName);

                    if (field != null)
                    {
                        try
                        {
                            field.SetValue(messageBase, value);
                        }
                        catch
                        {
                            // Might need to handle other generic types here...
                            if ((field.PropertyType.GetGenericTypeDefinition() == typeof(List<>)) &&
                                (value.Count > 0))
                            {
                                Type t = field.PropertyType;
                                var constructedListType = typeof(List<>).MakeGenericType(t);
                                IList instance = (IList)Activator.CreateInstance(t);
                                
                                foreach(object val in (IList)value)
                                {
                                    instance.Add(val);
                                }
                                
                                field.SetValue(messageBase, instance);
                            }
                        }
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("WARN: No attribute defined for {0} on class {1}", attributeName, messageType.Name));
                    }
                }
                else
                {
                    ((UnknownMessageType)messageBase).AttributeValues.Add(attributeName, value);
                }
            }

            Debug.WriteLine("Deserialized [{0}]: {1}", messageType, messageBase.ToString());

            return messageBase as IMessage;
        }

        public static byte[] Serialize(IMessage message)
        {
            var payload = WritePayloadFromMessage(message);
            var payloadShort = (short)(payload.Length + MESSAGE_SIZE_LENGTH_IN_BYTES);
            var payloadLength = BitConverter.GetBytes(payloadShort);

            var finalPayload = ByteHelper.CombineByteArray(payloadLength, payload);

            Debug.WriteLine("Serialized [{0}]: {1}", message.ToString(), BitConverter.ToString(finalPayload));

            return finalPayload;
        }

        private static byte[] WritePayloadFromMessage(IMessage message)
        {
            var propertyInfo = message.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var attributesToBeWritten = new List<Tuple<XMessageField, byte, dynamic>>();
            propertyInfo.Where(a => Attribute.IsDefined(a, typeof(XMessageField))).ToList()
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

            byte[] payload = { };
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
            payload = ms.ToArray();

            return payload;
        }
    }
}
