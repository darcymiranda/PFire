using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Inbound
{
    public class FriendRequestDecline : IMessage
    {
        [XFireAttributeDef("name")]
        public string RequesterUsername { get; private set; }

        public short MessageTypeId => 8;

        public void Process(Context context)
        {
            var requesterUser = context.Server.Database.QueryUser(RequesterUsername);
            var pendingRequests = context.Server.Database.QueryPendingFriendRequestsSelf(requesterUser);
            var requestsIds = pendingRequests.Where(a => a.UserId == requesterUser.UserId && a.FriendUserId == context.User.UserId)
                                             .Select(a => a.PendingFriendRequestId).ToArray();
            context.Server.Database.DeletePendingFriendRequest(requestsIds);
        }
    }
}
