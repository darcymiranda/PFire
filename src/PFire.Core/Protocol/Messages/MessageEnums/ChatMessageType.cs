namespace PFire.Core.Protocol.Messages.MessageEnums
{
    internal enum ChatMessageType : byte
    {
        Content = 0,
        Acknowledgement,
        ClientInformation,
        TypingNotification
    }
}
