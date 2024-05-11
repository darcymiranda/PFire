using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomJoinNotificationChanged : XFireMessage
    {
        public ChatroomJoinNotificationChanged(byte[] chatID, byte notifcationSetting) : base(XFireMessageType.ChatroomJoinNotificationChange)
        {
            ChatId = chatID;
            NotificationSetting = notifcationSetting;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x1B)]
        public byte NotificationSetting { get; set; }
    }
}
