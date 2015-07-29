using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Bidirectional
{
    public class ChatTypingNotification : IMessage
    {
        [XFireAttributeDef("imindex")]
        public int OrderIndex { get; private set; }

        [XFireAttributeDef("typing")]
        public int Typing { get; private set; }

        public short MessageTypeId
        {
            get { return 3; }
        }

        public void Process(Context context)
        {
            
        }
    }
}
