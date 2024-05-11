namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomVoiceSessionInfo : XFireMessage
    {
        public ChatroomVoiceSessionInfo(byte[] chatID, byte[] token) : base(XFireMessageType.ChatroomVoiceSessionInfo)
        {
            ChatId = chatID;
            Token = token;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x6B)]
        public byte[] Token { get; set; }
    }
}
