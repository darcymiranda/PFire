using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;
using FriendsList = PFire.Protocol.Messages.Outbound.FriendsList;

namespace PFire.Protocol.Messages.Inbound
{
    public class ConnectionInformation : IMessage
    {
        [XFireAttributeDef("conn")]
        public int Connection { get; private set; }

        [XFireAttributeDef("nat")]
        public int Nat { get; private set; }

        [XFireAttributeDef("naterr")]
        public int NatError { get; private set; }

        [XFireAttributeDef("sec")]
        public int Sec { get; private set; }

        [XFireAttributeDef("clientip")]
        public int ClientIp { get; private set; }

        [XFireAttributeDef("upnpinfo")]
        public string UpnpInfo { get; private set; }

        public short MessageTypeId => 17;

        public void Process(Context context)
        {
            var clientPrefs = new Unknown10();
            context.SendAndProcessMessage(clientPrefs);

            var groups = new Groups();
            context.SendAndProcessMessage(groups);

            var groupsFriends = new GroupsFriends();
            context.SendAndProcessMessage(groupsFriends);

            var serverList = new ServerList();
            context.SendAndProcessMessage(serverList);

            var chatRooms = new ChatRooms();
            context.SendAndProcessMessage(chatRooms);

            var friendsList = new FriendsList(context.User);
            context.SendAndProcessMessage(friendsList);

            var friendsStatus = new FriendsSessionAssign(context.User);
            context.SendAndProcessMessage(friendsStatus);

            // Tell friends this user came online
            //if (context.User.Username == "test") Debugger.Break();
            var friends = context.Server.Database.QueryFriends(context.User);
            friends.ForEach(friend =>
            {
                var otherSession = context.Server.GetSession(friend);
                otherSession?.SendAndProcessMessage(new FriendsSessionAssign(friend));
            });

            var pendingFriendRequests = context.Server.Database.QueryPendingFriendRequests(context.User);
            pendingFriendRequests.ForEach(request =>
            {
                var requester = context.Server.Database.QueryUser(request.UserId);
                context.SendAndProcessMessage(new FriendInvite(requester.Username, requester.Nickname, request.Message));
            });
        }
    }
}
