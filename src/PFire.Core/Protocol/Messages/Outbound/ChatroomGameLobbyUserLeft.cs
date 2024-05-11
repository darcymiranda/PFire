using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomGameLobbyUserLeft : XFireMessage
    {
        public ChatroomGameLobbyUserLeft(byte[] chatID, int userId) : base(XFireMessageType.GameLobbyUserLeft)
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
