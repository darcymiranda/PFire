using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class Did : IMessage
    {
        [XFireAttributeDef("did")]
        public byte[] Unknown { get; private set; }

        public short MessageTypeId
        {
            get { return 400; }
        }

        public void Process(Context context)
        {
            Unknown = new byte[] { 0xd1, 0xc2, 0x95, 0x33, 0x84, 0xc4, 0xcc, 0xb2, 0x31, 0x50, 0x1f, 0x0e, 0x43, 0xc5, 0x89, 0x30, 0xb2, 0xa7, 0x0e, 0x4a, 0xb6 };
        }
    }
}
