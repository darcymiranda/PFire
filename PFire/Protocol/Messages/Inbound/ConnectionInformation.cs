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

            var friendsStatus = new FriendsStatus(context.User);
            context.SendAndProcessMessage(friendsStatus);

            // Tell friends this user came online
            // TODO: Need to rethink design. FriendsStatus/FriendsList makes a lot of redudent calls to the database for friends
            //if (context.User.Username == "graaal") Debugger.Break();
            var friends = context.Server.Database.QueryFriends(context.User);
            friends.ForEach(user =>
            {
                var otherSession = context.Server.GetSession(user);
                if (otherSession != null)
                {
                    otherSession.SendAndProcessMessage(new FriendsStatus(user));
                }
            });
        }
    }
}
