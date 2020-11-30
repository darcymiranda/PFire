using PFire.Core.Protocol.Messages;

namespace PFire.Protocol.Messages.Bidirectional
{
    public sealed class ChatContent : XFireMessage
    {
        public ChatContent() : base(XFireMessageType.ServerChatMessage) { }

        [XMessageField("imindex")]
        public int MessageOrderIndex { get; }

        [XMessageField("im")]
        public string MessageContent { get; }
    }
}
