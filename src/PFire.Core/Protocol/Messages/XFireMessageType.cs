﻿namespace PFire.Core.Protocol.Messages
{
    public enum XFireMessageType : short
    {
        ChatContent = 0,
        LoginRequest = 1,
        UDPChatMessage = 2,
        ClientVersion = 3,
        GameInformation = 4,
        FriendsOfFriendsRequest = 5,
        FriendRequest = 6,
        FriendRequestAccept = 7,
        FriendRequestDecline = 8,
        FriendRemoval = 9,
        UserPreferences = 10,
        UserLookup = 12,
        KeepAlive = 13,
        NicknameChange = 14,
        ClientConfiguration = 16,
        ConnectionInformation = 17,
        GameServerFetchFriendsFavorites = 21,
        GameServerFetchAll = 22,
        StatusChange = 32,
        Unknown37 = 37,
        Logout = 36,
        LoginChallenge = 128,
        LoginFailure = 129,
        LoginSuccess = 130,
        FriendsList = 131,
        FriendsSessionAssign = 132,
        ServerChatMessage = 133,
        FriendsGameInfo = 135,
        FriendsOfFriendsInfo = 136,
        FriendInvite = 138,
        FriendRemoved = 139,
        ClientPreferences = 141,
        UserLookupResult = 143,
        ServerPong = 144,
        ServerList = 148,
        GameServerSendFriendsFavorites = 149,
        GameServerSendAll = 150,
        Groups = 151,
        GroupsFriends = 152,
        FriendStatusChange = 154,
        ChatRooms = 155,
        SystemBroadcast = 169,

        Did = 400
    }
}
