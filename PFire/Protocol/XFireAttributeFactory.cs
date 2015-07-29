using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.XFireAttributes;
using PFire.Protocol.Messages.Outbound;

namespace PFire.Protocol
{
    public class XFireAttributeFactory
    {
        private static XFireAttributeFactory instance;

        private Dictionary<byte, XFireAttribute> attributeTypes = new Dictionary<byte, XFireAttribute>();

        private XFireAttributeFactory()
        {
            Add(new StringAttribute());
            Add(new Int32Attribute());
            Add(new SessionIdAttribute());
            Add(new ListAttribute());
            Add(new DidAttribute());
            Add(new Int8KeyMapAttribute());
            Add(new StringKeyMapAttribute());
            Add(new Int8Attribute());
            Add(new MessageAttribute());
        }

        private void Add(XFireAttribute attributeValue)
        {
            attributeTypes.Add(
                attributeValue.AttributeTypeId,
                attributeValue
            );
        }

        public XFireAttribute GetAttribute(byte type)
        {
            if (!attributeTypes.ContainsKey(type))
            {
                throw new UnknownXFireAttributeTypeException(type);
            }
            return attributeTypes[type];
        }

        public XFireAttribute GetAttribute(Type type)
        {
            foreach (var keyValuePair in attributeTypes.ToList())
            {
                if (keyValuePair.Value.AttributeType.Name == type.Name)
                {
                    // Need to match on the first generic type for maps/dictionaries
                    if (type.GenericTypeArguments.Length > 1)
                    {
                        if (type.GenericTypeArguments.FirstOrDefault() != keyValuePair.Value.AttributeType.GenericTypeArguments.FirstOrDefault())
                        {
                            continue;
                        }
                    }

                    return GetAttribute(keyValuePair.Value.AttributeTypeId);
                }
            }
            throw new KeyNotFoundException(string.Format("XFireAttribute with type of {0} not found", type.Name));
        }

        public static XFireAttributeFactory Instance
        {
            get
            {
                return instance ?? new XFireAttributeFactory();
            }
        }
    }
}
