using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class ServerList : IMessage
    {
        [XFireAttributeDef("max")]
        public int MaximumFavourates { get; private set; }

        [XFireAttributeDef("gameid")]
        public List<int> GameIds { get; private set; }

        [XFireAttributeDef("gip")]
        public List<int> GameIPs { get; private set; }

        [XFireAttributeDef("gport")]
        public List<int> GamePorts { get; private set; }

        public short MessageTypeId
        {
            get { return 148; }
        }

        public void Process(Context context)
        {
            GameIds = new List<int>();
            GameIPs = new List<int>();
            GamePorts = new List<int>();
        }
    }
}
