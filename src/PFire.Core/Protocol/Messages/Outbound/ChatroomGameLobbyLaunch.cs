using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomGameLobbyLaunch : XFireMessage
    {
        public ChatroomGameLobbyLaunch(byte[] chatID, int launchStatus) : base(XFireMessageType.ChatroomGameLobbyLaunch)
        {
            ChatId = chatID;
            LaunchStatus = launchStatus;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x5a)]
        public int LaunchStatus { get; set; }
    }
}
