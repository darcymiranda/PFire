using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.XFireAttributes;

namespace PFire.Protocol
{
    class XFireAttributeDef : Attribute
    {
        public string Name { get; private set; }
        public byte[] NameAsBytes { get; private set; }
        public bool NonTextualName { get; private set; }

        public XFireAttributeDef(string name)
        {
            Name = name;
            NameAsBytes = Encoding.UTF8.GetBytes(name);
        }

        public XFireAttributeDef(params byte[] name)
            : this(Encoding.UTF8.GetString(name))
        {
            NonTextualName = true;
        }
    }
}
