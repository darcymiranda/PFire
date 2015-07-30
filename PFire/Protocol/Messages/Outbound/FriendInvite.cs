using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class FriendInvite : IMessage
    {
        [XFireAttributeDef("name")]
        public List<string> Usernames { get; private set; }

        [XFireAttributeDef("nick")]
        public List<string> Nicknames { get; private set; }

        [XFireAttributeDef("msg")]
        public List<string> Messages { get; private set; }

        public short MessageTypeId
        {
            get { return 138; }
        }

        public FriendInvite(string username, string nickname, string message)
        {
            Usernames = new List<string>();
            Nicknames = new List<string>();
            Messages = new List<string>();
            AddFriendInvitation(username, nickname, message);
        }

        public void AddFriendInvitation(string username, string nickname, string message)
        {
            Usernames.Add(username);
            Nicknames.Add(nickname);
            Messages.Add(message);
        }

        public void Process(Context context)
        {
        }
    }
}
