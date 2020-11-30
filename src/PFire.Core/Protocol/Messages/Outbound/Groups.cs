using PFire.Core.Protocol.Messages;
using PFire.Session;
using System.Collections.Generic;

namespace PFire.Protocol.Messages.Outbound
{
    public sealed class Groups : XFireMessage
    {
        public Groups() : base(XFireMessageType.Groups) { }

        [XMessageField(0x19)]
        public List<int> GroupIds { get; private set; }

        [XMessageField(0x1a)]
        public List<string> GroupNames { get; private set; }
         

        public override void Process(XFireClient context)
        {
            GroupIds = new List<int>();
            GroupNames = new List<string>();
        }
    }
}
