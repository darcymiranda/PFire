namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomMOTDChanged : XFireMessage
    {
        public ChatroomMOTDChanged(byte[] chatID, string motd) : base(XFireMessageType.ChatroomMOTDChanged)
        {
            ChatId = chatID;
            MOTD = motd;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x2E)]
        public string MOTD { get; set; }
    }
}
