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
            var clientPrefs = new Unknown10();
            await context.SendAndProcessMessage(clientPrefs);

            var groups = new Groups();
            await context.SendAndProcessMessage(groups);

            var groupsFriends = new GroupsFriends();
            await context.SendAndProcessMessage(groupsFriends);

            var serverList = new ServerList();
            await context.SendAndProcessMessage(serverList);

            var chatRooms = new ChatRooms();
            await context.SendAndProcessMessage(chatRooms);

            var friendsList = new FriendsList(context.User);
            await context.SendAndProcessMessage(friendsList);

            var friendsStatus = new FriendsSessionAssign(context.User);
            await context.SendAndProcessMessage(friendsStatus);

            // Tell friends this user came online
            //if (context.User.Username == "test") Debugger.Break();
            var friends = await context.Server.Database.QueryFriends(context.User);
            foreach (var friend in friends)
            {
                var otherSession = context.Server.GetSession(friend);
                if (otherSession != null)
                {
                    await otherSession.SendAndProcessMessage(new FriendsSessionAssign(friend));
                }
            }

            var pendingFriendRequests = await context.Server.Database.QueryPendingFriendRequests(context.User);
            foreach (var request in pendingFriendRequests)
            {
                var requester = await context.Server.Database.QueryUser(request.UserId);
                await context.SendAndProcessMessage(new FriendInvite(requester.Username, requester.Nickname, request.Message));
            }
        }
    }
}
