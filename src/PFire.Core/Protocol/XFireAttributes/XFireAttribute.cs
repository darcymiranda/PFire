using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.XFireAttributes
{
    public abstract class XFireAttribute
    {
        public abstract Type AttributeType { get; }
        public abstract byte AttributeTypeId { get; }
        public abstract dynamic ReadValue(BinaryReader reader);
        public abstract void WriteValue(BinaryWriter writer, dynamic data);

        public void WriteType(BinaryWriter writer)
        {
            writer.Write(AttributeTypeId);
        }
        public void WriteName(BinaryWriter writer, string name)
        {
            if (name == null) return;
            writer.Write((byte)name.Length);
            WriteNameWithoutLengthPrefix(writer, Encoding.UTF8.GetBytes(name));
            //writer.Write(Encoding.GetEncoding("ISO-8859-1").GetBytes(name));
        }

        public void WriteNameWithoutLengthPrefix(BinaryWriter writer, byte[] name)
        {
            writer.Write(name);
        }

        public void WriteAll(BinaryWriter writer, string name, dynamic data)
        {
            WriteName(writer, name);
            WriteType(writer);
            WriteValue(writer, data);
        }
    }
}
