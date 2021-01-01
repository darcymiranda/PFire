using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class Logout : XFireMessage
    {
        public Logout() : base(XFireMessageType.Logout) {}

        public override Task Process(IXFireClient client)
        {
            client.EndSession();
            return Task.CompletedTask;
        }
    }
}
