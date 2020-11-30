using System.Collections.Generic;
using PFire.Core.Session;
using PFire.Infrastructure.Database;

namespace PFire.Core.Protocol.Messages.Outbound
{
    public sealed class FriendsList : XFireMessage
    {
        private readonly User _ownerUser;

        public FriendsList(User owner)
            : base(XFireMessageType.FriendsList)
        {
            _ownerUser = owner;

            UserIds = new List<int>();
            Usernames = new List<string>();
            Nicks = new List<string>();
        }

        [XMessageField("userid")]
        public List<int> UserIds { get; private set; }

        [XMessageField("friends")]
        public List<string> Usernames { get; private set; }

        [XMessageField("nick")]
        public List<string> Nicks { get; private set; }

        public override void Process(XFireClient context)
        {
            var friends = context.Server.Database.QueryFriends(_ownerUser);
            friends.ForEach(f =>
            {
                UserIds.Add(f.UserId);
                Usernames.Add(f.Username);
                Nicks.Add(f.Nickname);
            });
        }
    }
}
