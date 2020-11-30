namespace PFire.Core.Protocol.Messages.Bidirectional
{
    public sealed class ChatAcknowledgement : XFireMessage
    {
        public ChatAcknowledgement() : base(XFireMessageType.ServerChatMessage) 
        {
        }
    }
}
