using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomDefaultPermsChanged : XFireMessage
    {
        public ChatroomDefaultPermsChanged(byte[] chatID, int perms) : base(XFireMessageType.ChatroomDefaultPermsChange)
        {
            ChatId = chatID;
            Perms = perms;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x13)]
        public int Perms { get; set; }
    }
}
