using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class KeepAlive : XFireMessage
    {
        public KeepAlive() : base(XFireMessageType.KeepAlive) {}

        public override Task Process(IXFireClient client)
        {
            return Task.CompletedTask;
        }
    }
}
