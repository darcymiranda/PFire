namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomPasswordChanged : XFireMessage
    {
        public ChatroomPasswordChanged(byte[] chatID, byte passwordEnabled) : base(XFireMessageType.ChatroomPasswordChanged)
        {
            ChatId = chatID;
            PasswordEnabled = passwordEnabled;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x5F)]
        public byte PasswordEnabled { get; set; }
    }
}
