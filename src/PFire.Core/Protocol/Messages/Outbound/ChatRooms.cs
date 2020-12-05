using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatRooms : XFireMessage
    {
        public ChatRooms() : base(XFireMessageType.ChatRooms)
        {
            ChatIds = new List<int>();
        }

        [XMessageField(0x04)]
        public List<int> ChatIds { get; }
    }
}
