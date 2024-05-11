using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomUserLeft : XFireMessage
    {
        public ChatroomUserLeft(byte[] chatID, int userId) : base(XFireMessageType.ChatroomUserLeft)
        {
            ChatId = chatID;
            UserId = userId;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x01)]
        public int UserId { get; set; }
    }
}
