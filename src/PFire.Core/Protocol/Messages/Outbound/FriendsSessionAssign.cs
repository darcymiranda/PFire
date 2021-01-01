using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendsSessionAssign : XFireMessage
    {
        private static readonly Guid FriendIsOffLineSessionId = Guid.Empty;
        private readonly UserModel _ownerUser;

        public FriendsSessionAssign(UserModel owner) : base(XFireMessageType.FriendsSessionAssign)
        {
            _ownerUser = owner;
            UserIds = new List<int>();
            SessionIds = new List<Guid>();
        }

        [XMessageField("userid")] 
        public List<int> UserIds { get; }

        [XMessageField("sid")] 
        public List<Guid> SessionIds { get; }

        [XMessageField(0x0b)] 
        public byte Unknown { get; set; }

        public override async Task Process(IXFireClient client)
        {
            // Client expects friends to be ordered by online first, then offline friends
            var friends = (await client.Server.Database.QueryFriends(_ownerUser))
                .Select(x => new {User = x, Session = client.Server.GetSession(x)})
                .OrderByDescending(x => x.Session != null).ToList();
            foreach (var friend in friends)
            {
                UserIds.Add(friend.User.Id);
                SessionIds.Add(friend.Session?.SessionId ?? FriendIsOffLineSessionId);
            }
        }
    }
}
