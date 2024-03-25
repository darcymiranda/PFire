using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class GroupsFriends : XFireMessage
    {
        public GroupsFriends() : base(XFireMessageType.GroupsFriends)
        {
            UserIds = new List<uint>();
            GroupIds = new List<uint>();
        }

        [XMessageField(0x01)]
        public List<uint> UserIds { get; }

        [XMessageField(0x19)]
        public List<uint> GroupIds { get; }
    }
}
