using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PFire.Core.Session;
using PFire.Infrastructure.Database;

namespace PFire.Core.Protocol.Messages.Outbound
{
    public sealed class FriendsSessionAssign : XFireMessage
    {
        private readonly User _ownerUser;

        public FriendsSessionAssign(User owner)
            : base(XFireMessageType.FriendsSessionAssign)
        {
            _ownerUser = owner;
            UserIds = new List<int>();
            SessionIds = new List<Guid>();
        }

        [XMessageField("userid")]
        public List<int> UserIds { get; private set; }

        [XMessageField("sid")]
        public List<Guid> SessionIds { get; private set; }

        [XMessageField(0x0b)]
        public byte Unknown { get; private set; }

        public override void Process(XFireClient client)
        {
            var friends = client.Server.Database.QueryFriends(_ownerUser);

            foreach (var friend in friends)
            {
                var friendSession = client.Server.GetSession(friend);

                UserIds.Add(friend.UserId);
                SessionIds.Add(friendSession != null ? friendSession.SessionId : Guid.Empty);
            }
        }
    }
}
