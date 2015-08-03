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

        public short MessageTypeId
        {
            get { return 8;  }
        }

        public void Process(Context context)
        {
            var requseterUser = context.Server.Database.QueryUser(RequesterUsername);
            var pendingRequests = context.Server.Database.QueryPendingFriendRequestsSelf(requseterUser);
            var requestsIds = pendingRequests.Where(a => a.UserId == requseterUser.UserId && a.FriendUserId == context.User.UserId)
                                             .Select(a => a.PendingFriendRequestId).ToArray();
            context.Server.Database.DeletePendingFriendRequest(requestsIds);
        }
    }
}
