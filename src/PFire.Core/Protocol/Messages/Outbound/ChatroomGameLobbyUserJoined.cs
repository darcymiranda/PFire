using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomGameLobbyUserJoined : XFireMessage
    {
        public ChatroomGameLobbyUserJoined(byte[] chatID, int userId) : base(XFireMessageType.GameLobbyUserJoined)
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
