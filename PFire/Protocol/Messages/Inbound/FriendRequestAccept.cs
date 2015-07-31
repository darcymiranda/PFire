using PFire.Protocol.Messages.Outbound;
using PFire.Session;
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

            context.Server.Database.InsertMutualFriend(context.User, friend);

            var friendsList = new FriendsList(context.User);
            context.SendAndProcessMessage(friendsList);

            // It's possible to accept a friend request where the inviter is not online
            var friendSession = context.Server.GetSession(friend);
            if (friendSession != null)
            {
                var otherFriendsList = new FriendsList(friend);
                friendSession.SendAndProcessMessage(otherFriendsList);
            }

            var pendingRequests = context.Server.Database.QueryPendingFriendRequests(context.User);
            var pq = pendingRequests.FirstOrDefault(a => a.FriendUserId == context.User.UserId);
            if (pq != null)
            {
                context.Server.Database.DeletePendingFriendRequest(pq.SequenceId);
            }
        }
    }
}
