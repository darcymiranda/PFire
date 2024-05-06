using System.Collections.Generic;
using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class KeepAlive : XFireMessage
    {
        public KeepAlive() : base(XFireMessageType.KeepAlive) { }

        [XMessageField("value")]
        public int? Value { get; set; }

        [XMessageField("stats")]
        public List<object> Stats { get; set; }

        public async override Task Process(IXFireClient client)
        {
            await client.SendAndProcessMessage(new ServerPong());
        }
    }
}
