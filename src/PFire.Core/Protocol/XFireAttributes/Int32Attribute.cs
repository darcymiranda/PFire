using System;
using System.IO;

namespace PFire.Core.Protocol.XFireAttributes
{
    class Int32Attribute : XFireAttribute
    {
        public override byte AttributeTypeId
        {
            get { return 0x02; }
        }
        public override Type AttributeType
        {
            get { return typeof(int); }
        }

        public override dynamic ReadValue(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            writer.Write((int)data);
        }
    }
}
