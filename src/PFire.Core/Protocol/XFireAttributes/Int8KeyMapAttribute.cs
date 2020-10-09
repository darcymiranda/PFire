using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.XFireAttributes;

namespace PFire.Protocol.XFireAttributes
{
    public class Int8KeyMapAttribute : MapAttribute<byte>
    {
        public override byte AttributeTypeId
        {
            get { return 0x09; }
        }
    }
}
