
using System.Text;
using System;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendNameChange : XFireMessage
    {
        public FriendNameChange(string nick, int userId) : base(XFireMessageType.FriendNameChange)
        {
            this.UserId = userId;
            this.Nick = nick;
        }

        [XMessageField(0x01)]
        public int UserId { get; set; }

        [XMessageField(0x0D)]
        public string Nick { get; set; }
    }
}
