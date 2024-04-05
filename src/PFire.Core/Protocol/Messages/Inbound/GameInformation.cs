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
            context.User.Game.Id = GameId;
            context.User.Game.Ip = GameIP;
            context.User.Game.Port = GamePort;
            await context.Server.SendGameInfoToFriends(context);
        }
    }
}
