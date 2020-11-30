using System;
using System.IO;

namespace PFire.Core.Protocol.XFireAttributes
{
    public class Int8Attribute : XFireAttribute
    {
        public override Type AttributeType
        {
            get { return typeof(byte); }
        }

        public override byte AttributeTypeId
        {
            get { return 0x08; }
        }

        public override dynamic ReadValue(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            writer.Write((byte)data);
        }
    }
}
