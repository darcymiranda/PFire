using PFire.Protocol.Messages.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Inbound
{
    public class FriendRequestAccept : IMessage
    {
        [XFireAttributeDef("name")]
        public string FriendUsername { get; private set; }

        public short MessageTypeId
        {
            get { return 7; }
        }

        public void Process(Context context)
        {
            var friend = context.Server.Database.QueryUser(FriendUsername);
            var friendSession = context.Server.GetSession(friend);
            context.Server.Database.InsertMutualFriend(context.User, friendSession.User);

            var friendsList = new FriendsList(context.User);
            context.SendAndProcessMessage(friendsList);

            var otherFriendsList = new FriendsList(friendSession.User);
            friendSession.SendAndProcessMessage(otherFriendsList);
        }
    }
}
