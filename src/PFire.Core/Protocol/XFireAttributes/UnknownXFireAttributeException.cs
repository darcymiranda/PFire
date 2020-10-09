using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.XFireAttributes
{
    public class UnknownXFireAttributeTypeException : Exception
    {
        public UnknownXFireAttributeTypeException(byte attributeTypeId) : base("Unknown xfire attribute type " + attributeTypeId) { }
    }
}
