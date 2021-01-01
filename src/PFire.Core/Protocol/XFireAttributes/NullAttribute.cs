// unset

using System;
using System.IO;

namespace PFire.Core.Protocol.XFireAttributes
{
    public class NullAttribute : XFireAttribute
    {
        public override byte AttributeTypeId => 0x00;

        public override Type AttributeType => typeof(string);
        public override dynamic ReadValue(BinaryReader reader)
        {
            return null;
        }

        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            // Do nothing
        }
    }
}
