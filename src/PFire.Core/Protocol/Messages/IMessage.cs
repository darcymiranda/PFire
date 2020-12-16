using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages
{
    internal interface IMessage
    {
        XFireMessageType MessageTypeId { get; }
        Task Process(IXFireClient client);
    }
}
