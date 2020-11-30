using PFire.Core.Protocol.Messages;

namespace PFire.Protocol.Messages.Bidirectional
{
    public sealed class ChatAcknowledgement : XFireMessage
    {
        public ChatAcknowledgement() : base(XFireMessageType.ServerChatMessage) 
        {
        }
    }
}
