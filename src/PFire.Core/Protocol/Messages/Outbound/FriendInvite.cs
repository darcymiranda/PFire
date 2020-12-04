using PFire.Core.Protocol.Messages;
using System.Collections.Generic;

namespace PFire.Protocol.Messages.Outbound
{
    public sealed class FriendInvite : XFireMessage
    {
        public FriendInvite() : base(XFireMessageType.FriendInvite) { }
        public FriendInvite(string username, string nickname, string message)
            : base(XFireMessageType.FriendInvite)
        {
            Usernames = new List<string>() { username };
            Nicknames = new List<string>() { nickname };
            Messages = new List<string>() { message };
        }

        [XMessageField("name")]
        public List<string> Usernames { get; private set; }

        [XMessageField("nick")]
        public List<string> Nicknames { get; private set; }

        [XMessageField("msg")]
        public List<string> Messages { get; private set; }
    }
}
