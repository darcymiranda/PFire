using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.XFireAttributes;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;

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

            var friendsList = new FriendsList();
            friendsList.Process(context);
            context.SendMessage(friendsList);

            var friendsStatus = new FriendsStatus();
            friendsStatus.Process(context);
            context.SendMessage(friendsStatus);
        }
    }
}
