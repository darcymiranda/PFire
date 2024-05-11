using System.Collections.Generic;
using PFire.Core.Session;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class GameServerSendFavorites : XFireMessage
    {
        public GameServerSendFavorites() : base(XFireMessageType.GameServerSendFavorites)
        {
            GameIds = new List<int>();
            GameIps = new List<int>();
            GamePorts = new List<int>();
        }

        [XMessageField("max")]
        public int MaximumFavorites { get; set; }

        [XMessageField("gameid")]
        public List<int> GameIds { get; }

        [XMessageField("gip")]
        public List<int> GameIps { get; }

        [XMessageField("gport")]
        public List<int> GamePorts { get; }

        public override async Task Process(IXFireClient context)
        {
            MaximumFavorites = 0x1e; //0x1e observed from an old pre-shutdown packet. I bet you can make it bigger, but I am keeping it to what xfire set it to.

            var servers = await context.Server.Database.GetAllUserFavoriteServers(context.User);

            foreach (var server in servers)
            {
                GameIds.Add(server.GameId);
                GameIps.Add(server.GameIp);
                GamePorts.Add(server.GamePort);
            }
        }
    }
}
