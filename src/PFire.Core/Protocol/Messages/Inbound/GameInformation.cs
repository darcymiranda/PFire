using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class GameInformation : XFireMessage
    {
        public GameInformation() : base(XFireMessageType.GameInformation) {}

        [XMessageField("gameid")]
        public int GameId { get; set; }

        [XMessageField("gip")]
        public int GameIP { get; set; }

        [XMessageField("gport")]
        public int GamePort { get; set; }

        public override async Task Process(IXFireClient context)
        {
            context.User.GameID = GameId;
            context.User.GameIP = GameIP;
            context.User.GamePort = GamePort;
            await context.UpdateGameInfo();
        }
    }
}
