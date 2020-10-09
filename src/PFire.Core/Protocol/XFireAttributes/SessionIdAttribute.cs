using PFire.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.XFireAttributes
{
    class SessionIdAttribute : XFireAttribute
    {
        public override byte AttributeTypeId
        {
            get { return 0x03; }
        }
        public override Type AttributeType
        {
            get { return typeof(Guid); }
        }

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
