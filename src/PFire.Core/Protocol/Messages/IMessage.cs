using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages
{
    public interface IMessage 
    {
        XFireMessageType MessageTypeId { get; }

        void Process(XFireClient client);
    }
}
