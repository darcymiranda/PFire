using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Bidirectional
{
    public class ChatContent : IMessage
    {
        [XFireAttributeDef("imindex")]
        public int MessageOrderIndex { get; private set; }

        [XFireAttributeDef("im")]
        public string MessageContent { get; private set; }

        public short MessageTypeId => 0;

        public void Process(Context context)
        {

        }
    }
}
