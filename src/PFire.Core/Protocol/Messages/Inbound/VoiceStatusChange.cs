using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using PFire.Infrastructure.Entities;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class VoiceStatusChange : XFireMessage
    {
        public VoiceStatusChange() : base(XFireMessageType.VoiceStatusChange) { }

        [XMessageField("vid")]
        public int VoiceId { get; set; }

        [XMessageField("vip")]
        public int VoiceIp { get; set; }

        [XMessageField("vport")]
        public int VoicePort { get; set; }

        public override async Task Process(IXFireClient context)
        {
            Guid sessionId = context.SessionId;
            var friends = await context.Server.Database.QueryFriends(context.User);
            foreach (var friend in friends)
            {
                var otherSession = context.Server.GetSession(friend);
                if (otherSession != null)
                {
                    await otherSession.SendAndProcessMessage(new FriendVoiceStatusChange(sessionId, VoiceId, VoiceIp, VoicePort));
                }
            }
        }
    }
}
