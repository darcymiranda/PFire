using System.Threading.Tasks;
using PFire.Core.Session;
using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.Messages.Outbound;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class KeepAlive : XFireMessage
    {
        public KeepAlive() : base(XFireMessageType.KeepAlive) {}

        public async override Task Process(IXFireClient client)
        {
            //Send a PONG response
            await client.SendAndProcessMessage(new ServerPong());
        }
    }
}
