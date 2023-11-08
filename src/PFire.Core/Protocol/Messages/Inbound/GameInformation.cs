using System.Threading.Tasks;
using PFire.Core.Session;
using PFire.Core.Protocol.Messages.Outbound;


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
            context.User.Game.GameID = GameId;
            context.User.Game.GameIP = GameIP;
            context.User.Game.GamePort = GamePort;
            await SendGameInfoToFriends(context);
        }
        public async Task SendGameInfoToFriends(IXFireClient context)
        {
            var friends = await context.Server.Database.QueryFriends(context.User);
            foreach (var friend in friends)
            {
                var otherSession = context.Server.GetSession(friend);
                if (otherSession != null)
                {
                    await otherSession.SendAndProcessMessage(new FriendsGamesInfo(context.User));
                }
            }
        }
    }
}
