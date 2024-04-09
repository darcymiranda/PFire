using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendVoiceStatusChange : XFireMessage
    {
        public FriendVoiceStatusChange(Guid sid, int voiceId, int voiceIp, int voicePort) : base(XFireMessageType.FriendVoiceStatusChange) { 
            SessionId = sid;
            VoiceId = voiceId;
            VoiceIp = voiceIp;
            VoicePort = voicePort;
        }
        
        [XMessageField("sid")]
        public Guid SessionId { get; set; }

        [XMessageField("vid")]
        public int VoiceId { get; set; }

        [XMessageField("vip")]
        public int VoiceIp { get; set; }

        [XMessageField("vport")]
        public int VoicePort { get; set; }
    }
}
