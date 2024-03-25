using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ServerList : XFireMessage
    {
        public ServerList() : base(XFireMessageType.ServerList)
        {
            GameIds = new List<uint>();
            GameIPs = new List<uint>();
            GamePorts = new List<uint>();
        }

        [XMessageField("max")]
        public uint MaximumFavorites { get; set; }

        [XMessageField("gameid")]
        public List<uint> GameIds { get; }

        [XMessageField("gip")]
        public List<uint> GameIPs { get; }

        [XMessageField("gport")]
        public List<uint> GamePorts { get; }
    }
}
