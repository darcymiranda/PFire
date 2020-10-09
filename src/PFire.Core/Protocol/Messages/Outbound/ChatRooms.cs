using PFire.Database;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class ChatRooms : IMessage
    {
        [XFireAttributeDef(0x04)]
        public List<int> ChatIds { get; private set; }

        public short MessageTypeId
        {
            get { return 155; }
        }

        public void Process(Context context)
        {
            ChatIds = new List<int>();
        }
    }
}
