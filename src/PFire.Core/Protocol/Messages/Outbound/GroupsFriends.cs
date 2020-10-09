using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class GroupsFriends : IMessage
    {
        [XFireAttributeDef(0x01)]
        public List<int> UserIds { get; private set; }

        [XFireAttributeDef(0x19)]
        public List<int> GroupIds { get; private set; }

    
        public short MessageTypeId
        {
            get { return 152; }
        }

        public void Process(Context context)
        {
            UserIds = new List<int>();
            GroupIds = new List<int>();
        }
    }
}