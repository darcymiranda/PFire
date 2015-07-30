using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.XFireAttributes;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;
using System.Diagnostics;

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

        public short MessageTypeId
        {
            get { return 17; }
        }

        public void Process(Context context)
        {
            var clientPrefs = new ClientPreferences();
            clientPrefs.Process(context);
            context.SendMessage(clientPrefs);

            var groups = new Groups();
            groups.Process(context);
            context.SendMessage(groups);

            var groupsFriends = new GroupsFriends();
            groupsFriends.Process(context);
            context.SendMessage(groupsFriends);

            var serverList = new ServerList();
            serverList.Process(context);
            context.SendMessage(serverList);

            var chatRooms = new ChatRooms();
            chatRooms.Process(context);
            context.SendMessage(chatRooms);

            var friendsList = new FriendsList(context.User);
            friendsList.Process(context);
            context.SendMessage(friendsList);

            var friendsStatus = new FriendsStatus(context.User);
            friendsStatus.Process(context);
            context.SendMessage(friendsStatus);

            // Tell friends this user came online
            // TODO: Need to rethink design. FriendsStatus/FriendsList makes a lot of redudent calls to the database for friends
            //if (context.User.Username == "graaal") Debugger.Break();
            var friends = context.Server.Database.QueryFriends(context.User);
            friends.ForEach(user =>
            {
                var otherSession = context.Server.GetSession(user);
                if (otherSession != null)
                {
                    var status = new FriendsStatus(user);
                    status.Process(context);
                    otherSession.SendMessage(status);
                }
            });
        }
    }
}
