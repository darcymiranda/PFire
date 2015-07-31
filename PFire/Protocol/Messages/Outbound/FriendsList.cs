using PFire.Database;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class FriendsList : IMessage
    {
        [XFireAttributeDef("userid")]
        public List<int> UserIds { get; private set; }

        [XFireAttributeDef("friends")]
        public List<string> Usernames { get; private set; }

        [XFireAttributeDef("nick")]
        public List<string> Nicks { get; private set; }

        public short MessageTypeId
        {
            get { return 131; }
        }

        private User owner;

        public FriendsList(User owner)
        {
            this.owner = owner;
            UserIds = new List<int>();
            Usernames = new List<string>();
            Nicks = new List<string>();
        }

        public void Process(Context context)
        {
            var friends = context.Server.Database.QueryFriends(owner);
            friends.ForEach(f =>
            {
                UserIds.Add(f.UserId);
                Usernames.Add(f.Username);
                Nicks.Add(f.Nickname);
            });
        }
    }
}
