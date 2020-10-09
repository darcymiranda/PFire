﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.XFireAttributes;

namespace PFire.Protocol.XFireAttributes
{
    // TODO: Should have its own type and not byte[]
    class DidAttribute : XFireAttribute
    {
        public override byte AttributeTypeId
        {
            get { return 0x06; }
        }
        public override Type AttributeType
        {
            get { return typeof(byte[]); }
        }

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
