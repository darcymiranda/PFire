using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.XFireAttributes
{
    public class StringAttribute : XFireAttribute
    {
        public override byte AttributeTypeId
        {
            get { return 0x01; }
        }

        public override Type AttributeType
        {
            get { return typeof(string); }
        }

        public override dynamic ReadValue(BinaryReader reader)
        {
            short valueLength = reader.ReadInt16();
            byte[] bytes = reader.ReadBytes(valueLength);
            return Encoding.UTF8.GetString(bytes);
        }
        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            string value = (string)data;
            if (value == null) value = String.Empty;

            writer.Write((short)value.Length);
            writer.Write(Encoding.UTF8.GetBytes(value));
        }
    }
}
