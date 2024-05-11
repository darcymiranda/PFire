using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendGameClientData : XFireMessage
    {
        public FriendGameClientData(List<Guid> sid, List<string> data) : base(XFireMessageType.FriendGameClientData)
        {
            SessionIds = sid;
            ClientData = data;
        }

        [XMessageField("sid")]
        public List<Guid> SessionIds { get; set; }

        [XMessageField("gcd")]
        public List<string> ClientData { get; set; }
    }
}
