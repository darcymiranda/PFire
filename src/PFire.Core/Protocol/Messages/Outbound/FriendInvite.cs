using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendInvite : XFireMessage
    {
        public FriendInvite(string username, string nickname, string message) : base(XFireMessageType.FriendInvite)
        {
            Usernames = new List<string>
            {
                username
            };

            Nicknames = new List<string>
            {
                nickname
            };

            Messages = new List<string>
            {
                message
            };
        }

        [XMessageField("name")]
        public List<string> Usernames { get; }

        [XMessageField("nick")]
        public List<string> Nicknames { get; }

        [XMessageField("msg")]
        public List<string> Messages { get; }
    }
}
