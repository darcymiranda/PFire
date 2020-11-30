using PFire.Core.Protocol.Messages;
using PFire.Database;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PFire.Protocol.Messages.Outbound
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

            // offline friends are just not sent
            var friendsSessions = friends.Select(a => client.Server.GetSession(a))
                                         .Where(a => a != null)
                                         .ToList();

            friendsSessions.ForEach(session =>
            {
                UserIds.Add(session.User.UserId);
                SessionIds.Add(session.SessionId);
                Debug.WriteLine("Status: For:{0} -- FriendId:{1} Friend:{2} FriendSession:{3}", client.User.Username, session.User.UserId, session.User.Username, session.SessionId);
            });
            
            //friendsSessions.OrderBy(a => a.User.UserId);
            //friendsSessions.ForEach(session => Debug.WriteLine("Context: For:{0} -- FriendId:{1} Friend:{2} FriendSession:{3}", context.User.Username, session.User.UserId, session.User.Username, session.SessionId));
        }
    }
}
