namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomSilenceChanged : XFireMessage
    {
        public ChatroomSilenceChanged(byte[] chatID, int userId, byte silenceSetting) : base(XFireMessageType.ChatroomSilenceChanged)
        {
            ChatId = chatID;
            UserId = userId;
            SilenceSetting = silenceSetting;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x01)]
        public int UserId { get; set; }
        [XMessageField(0x16)]
        public byte SilenceSetting { get; set; }
    }
}
