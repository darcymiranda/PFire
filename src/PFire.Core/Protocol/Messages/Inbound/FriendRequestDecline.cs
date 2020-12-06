﻿using System.Linq;
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
            var requesterUser = context.Server.Database.QueryUser(RequesterUsername);
            var pendingRequests = context.Server.Database.QueryPendingFriendRequestsSelf(requesterUser);

            var requestsIds = pendingRequests.Where(a => a.UserId == requesterUser.UserId && a.FriendUserId == context.User.UserId)
                                             .Select(a => a.PendingFriendRequestId)
                                             .ToArray();

            context.Server.Database.DeletePendingFriendRequest(requestsIds);
        }
    }
}
