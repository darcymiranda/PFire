using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomGameLobbyInfo : XFireMessage
    {
        public ChatroomGameLobbyInfo(byte[] chatID, int gameId, int gameHost, int gameIp, int gamePort, List<int> lobbyUsers) : base(XFireMessageType.ChatroomGameLobbyInfo)
        {
            ChatId = chatID;
            GameId = gameId;
            GameHost = gameHost;
            GameIp = gameIp;
            GamePort = gamePort;
            LobbyUsers = lobbyUsers;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x21)]
        public int GameId { get; set; }
        [XMessageField(0x01)]
        public int GameHost { get; set; }
        [XMessageField(0x22)]
        public int GameIp { get; set; }
        [XMessageField(0x23)]
        public int GamePort { get; set; }
        [XMessageField(0x48)]
        public List<int> LobbyUsers { get; set; }

    }
}
