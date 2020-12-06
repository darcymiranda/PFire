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
            var friend = await context.Server.Database.QueryUser(FriendUsername);

            await context.Server.Database.InsertMutualFriend(context.User, friend);

            await context.SendAndProcessMessage(new FriendsList(context.User));
            await context.SendAndProcessMessage(new FriendsSessionAssign(context.User));

            // It's possible to accept a friend request where the inviter is not online
            var friendSession = context.Server.GetSession(friend);
            if (friendSession != null)
            {
                await friendSession.SendAndProcessMessage(new FriendsList(friend));
                await friendSession.SendAndProcessMessage(new FriendsSessionAssign(friend));
            }

            var pendingRequests = await context.Server.Database.QueryPendingFriendRequests(context.User);
            var pq = pendingRequests.Where(a => a.ThemId == friend.Id).ToArray();
            await context.Server.Database.DeletePendingFriendRequest(pq);
        }
    }
}
