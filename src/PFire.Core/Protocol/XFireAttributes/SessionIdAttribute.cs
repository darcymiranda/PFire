using System;
using System.IO;

namespace PFire.Core.Protocol.XFireAttributes
{
    class SessionIdAttribute : XFireAttribute
    {
        public override byte AttributeTypeId => 0x03;

        public override Type AttributeType => typeof(Guid);

        public override dynamic ReadValue(BinaryReader reader)
        {
            return new Guid(reader.ReadBytes(16));
            //return new Guid(ByteHelper.ByteArrayToHexString(reader.ReadBytes(16)));
        }
        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            writer.Write(((Guid)data).ToByteArray());
            //writer.Write(ByteHelper.HexStringToByteArray(((Guid)data).ToString().Replace("-", "")));
        }
    }
}
