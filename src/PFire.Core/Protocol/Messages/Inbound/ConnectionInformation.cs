//#define CHATROOMMODE //Uncomment this line to enable Chat Room Mode (when you log in, everybody adds you automatically)
using System.Linq;
using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class ConnectionInformation : XFireMessage
    {
        public ConnectionInformation() : base(XFireMessageType.ConnectionInformation) {}

        [XMessageField("conn")]
        public int Connection { get; set; }

        [XMessageField("nat")]
        public int Nat { get; set; }

        [XMessageField("naterr")]
        public int NatError { get; set; }

        [XMessageField("sec")]
        public int Sec { get; set; }

        [XMessageField("clientip")]
        public int ClientIp { get; set; }

        [XMessageField("upnpinfo")]
        public string UpnpInfo { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var clientPrefs = new ClientPreferences();
            await context.SendAndProcessMessage(clientPrefs);

            var groups = new Groups();
            await context.SendAndProcessMessage(groups);

            var groupsFriends = new GroupsFriends();
            await context.SendAndProcessMessage(groupsFriends);

            var serverList = new ServerList();
            await context.SendAndProcessMessage(serverList);

            var chatRooms = new ChatRooms();
            await context.SendAndProcessMessage(chatRooms);

#if CHATROOMMODE
            var otherUsers = await context.Server.Database.AddEveryoneAsFriends(context.User);
#endif

            var friendsList = new FriendsList(context.User);
            await context.SendAndProcessMessage(friendsList);

            var friendsStatus = new FriendsSessionAssign(context.User);
            await context.SendAndProcessMessage(friendsStatus);

            //Grab all your friends and friends of friends games, dump them into a list and send it to you in one payload.
            var friendSessions = (await context.Server.Database.QueryFriends(context.User))
                                 .Union(await context.Server.Database.QueryFriendsOfFriends(context.User))
                                 .Distinct()
                                 .Select(friendId => context.Server.GetSession(friendId)?.User)
                                 .Where(friendUser => friendUser != null && friendUser.Id != context.User.Id)
                                 .ToList();

            await context.SendAndProcessMessage(new FriendsGamesInfo(friendSessions));

#if CHATROOMMODE    
            foreach (var otherUser in otherUsers)
            {
                var otherSession = context.Server.GetSession(otherUser);
                if (otherSession != null)
                {
                    await otherSession.SendAndProcessMessage(new FriendsList(otherSession.User));
                    await otherSession.SendMessage(
                        FriendsSessionAssign.UserCameOnline(context.User, context.SessionId));
                }
            }
#endif
        }
    }
}
