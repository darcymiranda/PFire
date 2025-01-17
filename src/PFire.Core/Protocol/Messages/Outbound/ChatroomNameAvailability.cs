using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomNameAvailability : XFireMessage
    {
        public ChatroomNameAvailability(int requestID, byte response) : base(XFireMessageType.ChatroomNameAvailability)
        {
            RequestId = requestID;
            Response = response;
        }

        [XMessageField(0x0b)]
        public int RequestId { get; set; }
        [XMessageField(0x0c)]
        public byte Response { get; set; }
    }
}
