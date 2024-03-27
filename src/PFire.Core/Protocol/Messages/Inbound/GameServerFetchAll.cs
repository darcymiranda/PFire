using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class GameServerFetchAll : XFireMessage
    {
        public GameServerFetchAll() : base(XFireMessageType.GameServerFetchAll) { }

        [XMessageField(0x21)]
        public int GameId { get; set; }

        public override async Task Process(IXFireClient context)
        {
            await context.SendAndProcessMessage(new GameServerSendAll(GameId));
        }
    }
}
