using System;
using System.IO;

namespace PFire.Core.Protocol.XFireAttributes
{
    public class SessionIdAttribute : XFireAttribute
    {
        public override byte AttributeTypeId => 0x03;

        public override Type AttributeType => typeof(Guid);

        public override dynamic ReadValue(BinaryReader reader)
        {
            return new Guid(reader.ReadBytes(16));
        }

        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            writer.Write(((Guid)data).ToByteArray());
        }
    }
}
