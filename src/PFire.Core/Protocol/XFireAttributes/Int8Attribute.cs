using PFire.Protocol.XFireAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.XFireAttributes
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
