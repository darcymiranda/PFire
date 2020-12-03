using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ServerList : XFireMessage
    {
        public ServerList() : base(XFireMessageType.ServerList)
        {
            GameIds = new List<int>();
            GameIPs = new List<int>();
            GamePorts = new List<int>();
        }

        [XMessageField("max")]
        public int MaximumFavorites { get; private set; }

        [XMessageField("gameid")]
        public List<int> GameIds { get; }

        [XMessageField("gip")]
        public List<int> GameIPs { get; }

        [XMessageField("gport")]
        public List<int> GamePorts { get; }
    }
}
