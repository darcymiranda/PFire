namespace PFire.Core.Protocol.Messages.Bidirectional
{
    // the typing notification is a sub message from the chat message and 
    // not a separate message in of itself

    internal sealed class ChatTypingNotification : XFireMessage
    {
        public ChatTypingNotification() : base(XFireMessageType.ServerChatMessage) {}

        [XMessageField("imindex")]
        public int OrderIndex { get; private set; }

        [XMessageField("typing")]
        public int Typing { get; private set; }
    }
}
