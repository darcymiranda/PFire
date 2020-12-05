using System.Linq;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class FriendRequestAccept : XFireMessage
    {
        public FriendRequestAccept() : base(XFireMessageType.FriendRequestAccept) {}

        [XMessageField("name")]
        public string FriendUsername { get; private set; }

        public override void Process(IXFireClient context)
        {
            var friend = context.Server.Database.QueryUser(FriendUsername);

            context.Server.Database.InsertMutualFriend(context.User, friend);

            context.SendAndProcessMessage(new FriendsList(context.User));
            context.SendAndProcessMessage(new FriendsSessionAssign(context.User));

            // It's possible to accept a friend request where the inviter is not online
            var friendSession = context.Server.GetSession(friend);
            if (friendSession != null)
            {
                friendSession.SendAndProcessMessage(new FriendsList(friend));
                friendSession.SendAndProcessMessage(new FriendsSessionAssign(friend));
            }

            var pendingRequests = context.Server.Database.QueryPendingFriendRequests(context.User);
            var pq = pendingRequests.FirstOrDefault(a => a.FriendUserId == context.User.UserId);
            if (pq != null)
            {
                context.Server.Database.DeletePendingFriendRequest(pq.PendingFriendRequestId);
            }
        }
    }
}
