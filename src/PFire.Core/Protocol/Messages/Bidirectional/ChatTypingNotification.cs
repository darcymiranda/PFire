using PFire.Core.Protocol.Messages;

namespace PFire.Protocol.Messages.Bidirectional
{
    // the typing notification is a sub message from the chat message and 
    // not a seperate message in of itself

    public sealed class ChatTypingNotification : XFireMessage
    {
        public ChatTypingNotification() : base(XFireMessageType.ServerChatMessage) { }

        [XMessageField("imindex")]
        public int OrderIndex { get; private set; }

        [XMessageField("typing")]
        public int Typing { get; private set; }
    }
}
