using System.Threading.Tasks;
using PFire.Core.Session;
using PFire.Core.Protocol.Messages.Outbound;


namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class GameInformation : XFireMessage
    {
        public GameInformation() : base(XFireMessageType.GameInformation) {}

        [XMessageField("gameid")]
        public uint GameId { get; set; }

        [XMessageField("gip")]
        public uint GameIP { get; set; }

        [XMessageField("gport")]
        public uint GamePort { get; set; }

        public override async Task Process(IXFireClient context)
        {
            context.User.Game.Id = GameId;
            context.User.Game.Ip = GameIP;
            context.User.Game.Port = GamePort;
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
