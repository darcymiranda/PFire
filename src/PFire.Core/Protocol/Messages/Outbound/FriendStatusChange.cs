using PFire.Core.Protocol.Messages;
using System;
using System.Collections.Generic;

namespace PFire.Protocol.Messages.Outbound
{
    public sealed class FriendStatusChange : XFireMessage
    {
        public FriendStatusChange() : base(XFireMessageType.FriendStatusChange) { }

        public FriendStatusChange(Guid sessionId, string message)
           : base(XFireMessageType.FriendStatusChange)
        {
            SessionIds = new List<Guid>() { sessionId };
            Messages = new List<string>() { message };

        }

        [XMessageField("sid")]
        public List<Guid> SessionIds { get; private set; }

        [XMessageField("msg")]
        public List<string> Messages { get; private set; }
    }
}
