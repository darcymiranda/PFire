using PFire.Core.Protocol.Messages;
using System.Collections.Generic;

namespace PFire.Protocol.Messages.Outbound
{
    public sealed class ServerList : XFireMessage
    {
        public ServerList() : base(XFireMessageType.ServerList) 
        {
            GameIds = new List<int>();
            GameIPs = new List<int>();
            GamePorts = new List<int>();
        }

        [XMessageField("max")]
        public int MaximumFavourates { get; private set; }

        [XMessageField("gameid")]
        public List<int> GameIds { get; private set; }

        [XMessageField("gip")]
        public List<int> GameIPs { get; private set; }

        [XMessageField("gport")]
        public List<int> GamePorts { get; private set; }
    }
}
