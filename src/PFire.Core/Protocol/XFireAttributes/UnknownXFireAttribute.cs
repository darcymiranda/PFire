using PFire.Protocol.XFireAttributes;
using System;
using System.IO;

namespace PFire.Core.Protocol.XFireAttributes
{
    class UnknownXFireAttribute : XFireAttribute
    {
        public override byte AttributeTypeId
        {
            get { return 0xFF; }
        }
        public override Type AttributeType
        {
            get { return typeof(object); }
        }

        public override dynamic ReadValue(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            writer.Write((int)data);
        }

        public byte TypeId { get; set; }


    }
}
