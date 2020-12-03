using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages
{
    internal interface IMessage
    {
        XFireMessageType MessageTypeId { get; }

        void Process(XFireClient client);
    }
}
