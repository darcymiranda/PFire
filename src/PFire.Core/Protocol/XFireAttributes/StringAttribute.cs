﻿using System;
using System.IO;
using System.Text;

namespace PFire.Core.Protocol.XFireAttributes
{
    public class StringAttribute : XFireAttribute
    {
        public override byte AttributeTypeId => 0x01;

        public override Type AttributeType => typeof(string);

        public override dynamic ReadValue(BinaryReader reader)
        {
            short valueLength = reader.ReadInt16();
            byte[] bytes = reader.ReadBytes(valueLength);
            return Encoding.UTF8.GetString(bytes);
        }
        public override void WriteValue(BinaryWriter writer, dynamic data)
        {
            string value = (string)data ?? string.Empty;

            writer.Write((short)value.Length);
            writer.Write(Encoding.UTF8.GetBytes(value));
        }
    }
}
