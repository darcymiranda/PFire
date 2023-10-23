using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Session;

/// <summary>
/// Packet 144 - Server to Client PONG response
/// Attributes:
///     value - Always zero, I suspect that they just need one attribute.
/// </summary>

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ServerPong : XFireMessage
    {
        public ServerPong() : base(XFireMessageType.ServerPong) { }

        [XMessageField("value")]
        public int Value { get; set; }

        public override Task Process(IXFireClient context)
        {
            return Task.CompletedTask;
        }
    }
}
