namespace PFire.Core.Protocol.Messages
{
    public enum XFireMessageType : short
    {
        ChatContent = 0,
        LoginRequest = 1,
        UDPChatMessage = 2,
        ClientVersion = 3,
        GameInformation = 4,
        FriendRequest = 6,
        FriendRequestAccept = 7,
        FriendRequestDecline = 8,
        Unknown10 = 10,
        UserLookup = 12,
        KeepAlive = 13,
        NicknameChange = 14,
        ClientConfiguration = 16,
        ConnectionInformation = 17,
        StatusChange = 32,
        Unknown37 = 37,
        Logout = 36,
        LoginChallenge = 128,
        LoginFailure = 129,
        LoginSuccess = 130,
        FriendsList = 131,
        FriendsSessionAssign = 132,
        ServerChatMessage = 133,
        FriendInvite = 138,
        ClientPreferences = 141,
        UserLookupResult = 143,
        ServerList = 148,
        Groups = 151,
        GroupsFriends = 152,
        FriendStatusChange = 154,
        ChatRooms = 155,

        Did = 400
    }
}
