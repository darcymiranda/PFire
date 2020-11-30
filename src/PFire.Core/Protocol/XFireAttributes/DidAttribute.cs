﻿using System;
using System.IO;

namespace PFire.Core.Protocol.XFireAttributes
{
    // TODO: Should have its own type and not byte[]
    class DidAttribute : XFireAttribute
    {
        public override byte AttributeTypeId => 0x06;

        public override Type AttributeType => typeof(byte[]);

        public override dynamic ReadValue(BinaryReader reader)
        {
            return reader.ReadBytes(21);
        }
        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            writer.Write((byte[])data);
        }
    }
}
