﻿using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;

namespace PFire.Protocol.Messages.Inbound
{
    public sealed class ConnectionInformation : XFireMessage
    {
        public ConnectionInformation() : base(XFireMessageType.ConnectionInformation) { }

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

        public override void Process(XFireClient context)
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
