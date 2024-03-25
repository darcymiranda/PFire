using System;
using System.IO;

namespace PFire.Core.Protocol.XFireAttributes
{
    public class UInt32Attribute : XFireAttribute
    {
        public override byte AttributeTypeId => 0x02;

        public override Type AttributeType => typeof(uint);

        public override dynamic ReadValue(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            writer.Write((uint)data);
        }
    }
}
