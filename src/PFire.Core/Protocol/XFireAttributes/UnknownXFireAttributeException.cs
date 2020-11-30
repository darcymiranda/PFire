using System;

namespace PFire.Core.Protocol.XFireAttributes
{
    public class UnknownXFireAttributeTypeException : Exception
    {
        public UnknownXFireAttributeTypeException(byte attributeTypeId) : base("Unknown xfire attribute type " + attributeTypeId) { }
    }
}
