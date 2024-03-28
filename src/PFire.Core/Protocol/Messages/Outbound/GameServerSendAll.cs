using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class GameServerSendAll : XFireMessage
    {
        public GameServerSendAll(int gameId) : base(XFireMessageType.GameServerSendAll)
        {
            GameId = gameId;
            GameIps = new List<int>();
            GamePorts = new List<int>();
        }

        [XMessageField(0x21)]
        public int GameId { get; set; }

        [XMessageField(0x22)]
        public List<int> GameIps { get; set; }

        [XMessageField(0x23)]
        public List<int> GamePorts { get; set; }

        public override Task Process(IXFireClient context)
        {
            //TODO: Have a Database of IPs and Ports that is fetched by gameid
            //      Send back the GameId sent
            //      Iterate that into Ips and Ports (unsigned ints on both)
            //      If no hits, send with empty List<int>s regardless, because the client expects a response.

            return Task.CompletedTask;
        }
    }
}
