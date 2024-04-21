using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomVisibilityChanged : XFireMessage
    {
        public ChatroomVisibilityChanged(byte[] chatID, int visibility) : base(XFireMessageType.ChatroomVisibilityChanged)
        {
            ChatId = chatID;
            Visibility = visibility;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x17)]
        public int Visibility { get; set; }
    }
}
