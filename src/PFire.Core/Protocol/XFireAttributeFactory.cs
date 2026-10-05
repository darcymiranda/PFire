using System;
using System.Collections.Generic;
using System.Linq;
using PFire.Core.Protocol.XFireAttributes;

namespace PFire.Core.Protocol
{
    public class XFireAttributeFactory
    {
        private static readonly XFireAttributeFactory instance = null;

        private readonly Dictionary<byte, XFireAttribute> _attributeTypes = new Dictionary<byte, XFireAttribute>();

        private static readonly byte[] IgnoreKnownUnimplementedTypes = {18};

        private XFireAttributeFactory()
        {
            Add(new StringAttribute());
            //Add(new Int32Attribute());
            Add(new UInt32Attribute());
            Add(new SessionIdAttribute());
            Add(new ListAttribute());
            Add(new DidAttribute());
            Add(new Int8KeyMapAttribute());
            Add(new StringKeyMapAttribute());
            Add(new Int8Attribute());
            Add(new MessageAttribute());
            Add(new NullAttribute());
        }

        private void Add(XFireAttribute attributeValue)
        {
            _attributeTypes.Add(attributeValue.AttributeTypeId, attributeValue);
        }

        public XFireAttribute GetAttribute(byte type)
        {
            if (_attributeTypes.TryGetValue(type, out var xFireAttribute))
            {
                return xFireAttribute;
            }

            // Avoid having to implement these attribute types to continue processing other requests instead of
            // throwing an exception
            if (IgnoreKnownUnimplementedTypes.Contains(type))
            {
                return new NullAttribute();
            }

            throw new UnknownXFireAttributeTypeException(type);
        }

        public XFireAttribute GetAttribute(Type type)
        {
            foreach (var keyValuePair in _attributeTypes.Where(x => x.Value.AttributeType.Name == type.Name))
            {
                // Need to match on the first generic type for maps/dictionaries
                if (type.GenericTypeArguments.Length > 1)
                {
                    if (type.GenericTypeArguments.FirstOrDefault() !=
                        keyValuePair.Value.AttributeType.GenericTypeArguments.FirstOrDefault())
                    {
                        continue;
                    }
                }

                return GetAttribute(keyValuePair.Value.AttributeTypeId);
            }

            throw new KeyNotFoundException($"XFireAttribute with type of {type.Name} not found");
        }

        public static XFireAttributeFactory Instance => instance ?? new XFireAttributeFactory();
    }
}
