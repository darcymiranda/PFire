using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomUserPermsChanged : XFireMessage
    {
        public ChatroomUserPermsChanged(byte[] chatID, int userId, int perms) : base(XFireMessageType.ChatroomUserPermsChanged)
        {
            ChatId = chatID;
            UserId = userId;
            Permissions = perms;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x18)]
        public int UserId { get; set; }
        [XMessageField(0x13)]
        public int Permissions { get; set; }
    }
}
