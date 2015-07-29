using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class FriendsList : IMessage
    {
        [XFireAttributeDef("userid")]
        public List<int> UserId { get; private set; }

        [XFireAttributeDef("friends")]
        public List<string> Friends { get; private set; }

        [XFireAttributeDef("nick")]
        public List<string> Nick { get; private set; }

        public short MessageTypeId
        {
            get { return 131; }
        }

        public void Process(Context context)
        {
            var user = context.Server.Database.QueryUser(1);
            var user2 = context.Server.Database.QueryUser(2);

            UserId = new List<int>();
            UserId.Add(user.UserId);
            UserId.Add(user2.UserId);
            Friends = new List<string>();
            Friends.Add(user.Username);
            Friends.Add(user2.Username);
            Nick = new List<string>();
            Nick.Add(user.Nickname);
            Nick.Add(user2.Nickname);
        }
    }
}
