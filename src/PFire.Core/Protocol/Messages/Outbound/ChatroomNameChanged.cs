using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomNameChanged : XFireMessage
    {
        public ChatroomNameChanged(byte[] chatID, string chatName) : base(XFireMessageType.ChatroomNameChanged)
        {
            ChatId = chatID;
            ChatName = chatName;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x05)]
        public string ChatName { get; set; }
    }
}
