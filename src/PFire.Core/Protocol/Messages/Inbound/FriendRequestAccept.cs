using System.Linq;
using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class FriendRequestAccept : XFireMessage
    {
        public FriendRequestAccept() : base(XFireMessageType.FriendRequestAccept) {}

        [XMessageField("name")]
        public string FriendUsername { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var friend = context.Server.Database.QueryUser(FriendUsername);

            context.Server.Database.InsertMutualFriend(context.User, friend);

            await context.SendAndProcessMessage(new FriendsList(context.User));
            await context.SendAndProcessMessage(new FriendsSessionAssign(context.User));

            // It's possible to accept a friend request where the inviter is not online
            var friendSession = context.Server.GetSession(friend);
            if (friendSession != null)
            {
                await friendSession.SendAndProcessMessage(new FriendsList(friend));
                await friendSession.SendAndProcessMessage(new FriendsSessionAssign(friend));
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
