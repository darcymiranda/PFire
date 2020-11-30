using System;
using System.Text;

namespace PFire.Protocol
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    internal sealed class XMessageField : Attribute
    {
        public string Name { get; }
        public byte[] NameAsBytes { get; }
        public bool NonTextualName { get; }

        public XMessageField(string name)
        {
            Name = name;
            NameAsBytes = Encoding.UTF8.GetBytes(name);
        }

        public XMessageField(params byte[] name)
            : this(Encoding.UTF8.GetString(name))
        {
            NonTextualName = true;
        }
    }
}
