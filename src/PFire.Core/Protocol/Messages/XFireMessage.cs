using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages
{
    internal abstract class XFireMessage : IMessage
    {
        protected XFireMessage(XFireMessageType typeId)
        {
            MessageTypeId = typeId;
        }

        public XFireMessageType MessageTypeId { get; }

        public virtual Task Process(IXFireClient client)
        {
            // base implementation is to do nothing
            client.Logger.LogWarning($" *** Unimplemented processing for message type {MessageTypeId}");

            return Task.CompletedTask;
        }
    }
}
