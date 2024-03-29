using System;
using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class SystemBroadcastMessage : XFireMessage
    {
        public SystemBroadcastMessage(string message) : base(XFireMessageType.SystemBroadcast)
        {
            Message = message;
        }

        // Not entirely sure what this does. I looked at decompiled code and it seems to make it
        // skip over reading a string if set to zero or something. However even when set to zero
        // it still displays.
        [XMessageField(0x34)]
        public int Unk { get; set; }

        [XMessageField(0x2E)]
        public string Message { get; set; }
    }
}
