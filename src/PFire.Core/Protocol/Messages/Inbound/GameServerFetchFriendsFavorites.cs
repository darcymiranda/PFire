using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class GameServerFetchFriendsFavorites : XFireMessage
    {
        public GameServerFetchFriendsFavorites() : base(XFireMessageType.GameServerFetchFriendsFavorites) { }

        [XMessageField("gameid")]
        public int GameId { get; set; }

        public override async Task Process(IXFireClient context)
        {
            await context.SendAndProcessMessage(new GameServerSendFriendsFavorites(GameId));
        }
    }
}
