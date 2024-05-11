using PFire.Core.Models;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatRoomInvitationDenied : XFireMessage
    {
        public ChatRoomInvitationDenied(byte[] cid) : base(XFireMessageType.ChatroomInvitationDenied)
        {
            ChatId = cid;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
    }
}
