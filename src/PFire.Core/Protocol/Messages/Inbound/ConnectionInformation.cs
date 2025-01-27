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
            var clientPrefs = new UserPreferences();
            await context.SendAndProcessMessage(clientPrefs);

            var groups = new Groups();
            await context.SendAndProcessMessage(groups);

            var groupsFriends = new GroupsFriends();
            await context.SendAndProcessMessage(groupsFriends);

            var serverList = new ServerList();
            await context.SendAndProcessMessage(serverList);

            var chatRooms = new ChatRooms();
            await context.SendAndProcessMessage(chatRooms);
            
            // TODO: Remove chat room mode
            var otherUsers = await context.Server.Database.AddEveryoneAsFriends(context.User);

            var friendsList = new FriendsList(context.User);
            await context.SendAndProcessMessage(friendsList);

            var friendsStatus = new FriendsSessionAssign(context.User);
            await context.SendAndProcessMessage(friendsStatus);
            
            // TODO: Remove chat room mode
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
        }
    }
}
