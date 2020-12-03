namespace PFire.Core.Protocol.Messages.Bidirectional
{
    internal sealed class ChatAcknowledgement : XFireMessage
    {
        public ChatAcknowledgement() : base(XFireMessageType.ServerChatMessage) {}
    }
}
