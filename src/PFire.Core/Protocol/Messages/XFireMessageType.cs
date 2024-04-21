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
        VoiceStatusChange = 15,
        ClientConfiguration = 16,
        ConnectionInformation = 17,
        FavoriteServerAdd = 19,
        FavoriteServerRemove = 20,
        GameServerFetchFriendsFavorites = 21,
        GameServerFetchAll = 22,
        ChatroomInbound = 25,
        GroupCreate = 26,
        GroupRemove = 27,
        GroupRename = 28,
        GroupMemberAdd = 29,
        GroupMemberRemove = 30,
        StatusChange = 32,
        GameClientData = 35,
        Logout = 36,
        Unknown37 = 37,
        LoginChallenge = 128,
        LoginFailure = 129,
        LoginSuccess = 130,
        FriendsList = 131,
        FriendsSessionAssign = 132,
        ServerChatMessage = 133,
        FriendsGameInfo = 135,
        FriendsOfFriendsInfo = 136,
        FriendInviteResponse = 137,
        FriendInvite = 138,
        FriendRemoved = 139,
        ClientPreferences = 141,
        UserLookupResult = 143,
        ServerPong = 144,
        LoggedInElseWhere = 145,
        FriendVoiceStatusChange = 147,
        GameServerSendFavorites = 148,
        GameServerSendFriendsFavorites = 149,
        GameServerSendAll = 150,
        Groups = 151,
        GroupsFriends = 152,
        GroupCreateConfirmation = 153,
        FriendStatusChange = 154,
        ChatRooms = 155,
        FriendGameClientData = 156,
        UserScreenshots = 157,
        FriendNameChange = 161,
        SystemBroadcast = 169,
        ChatroomNameChanged = 350,
        ChatRoomJoinInfo = 351,
        ChatroomUserJoined = 353,
        ChatroomUserLeft = 354,
        ChatroomMessage = 355,
        ChatroomInvitationSent = 356,
        ChatroomUserPermsChanged = 357,
        ChatroomInfoOnLogin = 358,
        ChatroomUserKicked = 359,
        ChatroomVoiceStatusChange = 360,
        ChatroomVoiceHostInfo = 363,
        ChatroomGameLobbyInfo = 366,
        ChatroomFriendList = 368,
        ChatroomDefaultPermsChange = 370,
        GameLobbyUserJoined = 372,
        GameLobbyUserLeft = 373,
        ChatroomMOTDChanged = 374,
        ChatroomGameLobbyLaunch = 377,
        ChatroomVoiceSessionInfo = 383,
        ChatroomNameAvailability = 384,
        ChatroomPasswordChanged = 385,
        ChatroomVisibilityChanged = 386,
        ChatroomInvitationDenied = 387,
        ChatroomSilenceChanged = 388,
        ChatroomJoinNotificationChange = 389,

        Did = 400
    }
}
