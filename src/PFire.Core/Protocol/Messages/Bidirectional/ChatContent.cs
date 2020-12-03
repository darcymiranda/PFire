namespace PFire.Core.Protocol.Messages.Bidirectional
{
    internal sealed class ChatContent : XFireMessage
    {
        public ChatContent() : base(XFireMessageType.ServerChatMessage) {}

        [XMessageField("imindex")]
        public int MessageOrderIndex { get; }

        [XMessageField("im")]
        public string MessageContent { get; }
    }
}
