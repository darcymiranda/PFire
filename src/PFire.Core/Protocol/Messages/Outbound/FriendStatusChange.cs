using System;
using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendStatusChange : XFireMessage
    {
        public FriendStatusChange(Guid sessionId, string message) : base(XFireMessageType.FriendStatusChange)
        {
            SessionIds = new List<Guid>
            {
                sessionId
            };

            Messages = new List<string>
            {
                message
            };
        }

        [XMessageField("sid")]
        public List<Guid> SessionIds { get; }

        [XMessageField("msg")]
        public List<string> Messages { get; }
    }
}
