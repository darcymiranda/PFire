using PFire.Core.Protocol.Messages;
using PFire.Session;

namespace PFire.Protocol.Messages
{
    public interface IMessage 
    {
        XFireMessageType MessageTypeId { get; }

        void Process(XFireClient client);
    }
}
