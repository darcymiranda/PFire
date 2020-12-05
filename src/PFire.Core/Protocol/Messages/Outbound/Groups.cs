using System.Collections.Generic;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class Groups : XFireMessage
    {
        public Groups() : base(XFireMessageType.Groups) {}

        [XMessageField(0x19)]
        public List<int> GroupIds { get; private set; }

        [XMessageField(0x1a)]
        public List<string> GroupNames { get; private set; }

        public override void Process(IXFireClient context)
        {
            GroupIds = new List<int>();
            GroupNames = new List<string>();
        }
    }
}
