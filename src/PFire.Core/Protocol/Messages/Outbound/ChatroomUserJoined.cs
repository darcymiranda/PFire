using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomUserJoined : XFireMessage
    {
        public ChatroomUserJoined(byte[] chatID, UserModel user, int perms) : base(XFireMessageType.ChatroomUserJoined)
        {
            ChatId = chatID;
            UserId = user.Id;
            Username = user.Username;
            Nickname = user.Nickname;
            Permission = perms;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x01)]
        public int UserId { get; set; }
        [XMessageField(0x02)]
        public string Username { get; set; }
        [XMessageField(0x0d)]
        public string Nickname { get; set; }
        [XMessageField(0x12)]
        public int Permission { get; set; }
    }
}
