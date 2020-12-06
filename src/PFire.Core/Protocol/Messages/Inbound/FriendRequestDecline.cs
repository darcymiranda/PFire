using System.Linq;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class FriendRequestDecline : XFireMessage
    {
        public FriendRequestDecline() : base(XFireMessageType.FriendRequestDecline) {}

        [XMessageField("name")]
        public string RequesterUsername { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var requesterUser = await context.Server.Database.QueryUser(RequesterUsername);
            var pendingRequests = await context.Server.Database.QueryPendingFriendRequestsSelf(requesterUser);

            var requestsIds = pendingRequests.Where(a => a.MeId == requesterUser.Id && a.ThemId == context.User.Id).ToArray();

            await context.Server.Database.DeletePendingFriendRequest(requestsIds);
        }
    }
}
