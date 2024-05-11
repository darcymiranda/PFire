namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomUserKicked : XFireMessage
    {
        public ChatroomUserKicked(byte[] chatID, int userId) : base(XFireMessageType.ChatroomUserKicked)
        {
            ChatId = chatID;
            UserId = userId;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x18)]
        public int UserId { get; set; }
    }
}
