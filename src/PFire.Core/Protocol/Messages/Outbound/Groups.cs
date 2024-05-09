using System.Collections.Generic;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class Groups : XFireMessage
    {
        public Groups() : base(XFireMessageType.Groups) {}

        [XMessageField(0x19)]
        public List<uint> GroupIds { get; set; }

        [XMessageField(0x1a)]
        public List<string> GroupNames { get; set; }

        public override Task Process(IXFireClient context)
        {
            GroupIds = new List<uint>();
            GroupNames = new List<string>();

            return Task.CompletedTask;
        }
    }
}
