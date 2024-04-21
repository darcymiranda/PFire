using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PFire.Core.Models;
using PFire.Core.Protocol.Messages.Inbound;
using PFire.Infrastructure.Entities;
using PFire.Infrastructure.Services;

namespace PFire.Core.Services
{
    internal interface IPFireDatabase
    {
        // User and Friend List Functions
        Task<IList<UserModel>> AddEveryoneAsFriends(UserModel user);
        Task<UserModel> InsertUser(string username, string password, string salt);
        Task InsertMutualFriend(UserModel me, UserModel them);
        Task InsertFriendRequest(UserModel me, UserModel them, string message);
        Task<UserModel> QueryUser(string username);
        Task<UserModel> QueryUser(int userId);
        Task<List<UserModel>> QueryUsers(string username);
        Task<List<UserModel>> QueryUsers(List<int> userIds);
        Task<List<UserModel>> QueryFriends(UserModel user);
        Task<List<UserModel>> QueryFriendsOfFriends(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequestsSelf(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequests(UserModel otherUser);
        Task DeletePendingFriendRequest(UserModel me, params FriendRequestModel[] thems);
        Task UpdateNickname(UserModel user, string nickname);
        Task RemoveFriend(UserModel me, UserModel them);
        Task SaveUserPrefs(UserModel user);
        // User Group Functions
        Task<UserGroup> CreateUserGroup(UserModel user, string groupName);
        Task<UserGroup> GetUserGroup(int groupId);
        Task<UserGroup> GetUserGroup(string groupName);
        Task<List<UserGroup>> GetAllUserGroups(UserModel user);
        Task RemoveUserGroup(int groupId);
        Task<List<int>> GetUserGroupMembers(int groupId);
        Task RenameUserGroup(int groupId, string newName);
        Task AddMemberToUserGroup(int groupId, int userId);
        Task RemoveMemberFromUserGroup(int groupId, int userId);
        // User Favorite Server Functions
        Task<List<UserServerList>> GetAllUserFavoriteServers (UserModel user);
        Task AddUserFavoriteServer (int gameId, int gameIp, int gamePort, int userId);
        Task RemoveUserFavoriteServer (int gameId, int gameIp, int gamePort, int userId);
        // Chatroom Functions
        Task<ChatroomModel> InsertChatroom(ChatroomModel chatroom);
        Task<ChatroomModel> QueryChatroom(byte[] cid);
        Task<ChatroomModel> QueryChatroom(string topic);
        Task<List<byte[]>> GetChatrooms();
        Task<List<ChatroomModel>> GetAllChatrooms();
        Task<List<ChatroomModel>> GetAllVisibleChatrooms();
        Task<string> GetChatroomName(byte[] cid);
        Task<List<int>> QueryCurrentChatroomUsers(byte[] cid);
        Task<int> GetUserHighestChatPowerLevel(byte[] roomId, int userId);
        Task<List<(int UserId, int HighestLevel)>> GetUsersHighestChatPowerLevels(byte[] roomId, List<int> userIds);
        Task UserEnteredRoom(byte[] roomId, int userid);
        Task<int> UserLeftRoom(byte[] roomId, int userid);
        Task UpdateChatroomName(byte[] roomId, string name);
        Task UpdateChatroomUserPerms(byte[] roomId, int userId, int perms);
        Task UpdateChatroomSaveStatus(byte[] roomId, byte saveToggle);
        Task UpdateChatroomSilenceStatus(byte[] roomId, byte silenceToggle);
        Task<bool> IsRoomSilenced(byte[] roomId);
        Task UpdateChatroomMOTD(byte[] roomId, string motd);
        Task UpdateChatroomJoinNotification(byte[] roomId, byte notificationToggle);
        Task UpdateChatroomVisibility(byte[] roomId, int visibility);
        Task UpdateChatroomPassword(byte[] cid, string password);
        Task KickChatroomUser(byte[] roomId, int userId);
        Task UpdateChatroomDefaultPerms(byte[] roomId, int perms);
        Task<List<UserModel>> QueryUsersInChat(byte[] cid);
        // Chatroom Game Lobby Functions
        Task StartChatroomGameLobby(byte[] cid, int gameId, int gameIp, int gamePort, List<int> lobbyUsers);
        Task UserJoinedGameLobby(byte[] roomId, int userid);
        Task UserLeftGameLobby(byte[] roomId, int userid);
        Task<List<UserModel>> QueryGameLobbyUsersInChat(byte[] cid);
    }

    internal class PFireDatabase : IPFireDatabase
    {
        private readonly IServiceProvider _serviceProvider;

        public PFireDatabase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IList<UserModel>> AddEveryoneAsFriends(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var myFriendIds = await databaseContext.Set<Friend>()
                .Where(x => x.MeId == user.Id)
                .Select(y => y.ThemId)
                .ToListAsync();

            var otherUsers = await databaseContext.Set<User>()
                .Where(x => x.Id != user.Id)
                .Where(x => !myFriendIds.Contains(x.Id))
                .Select(x => new UserModel { Id = x.Id})
                .ToListAsync();
            
            foreach (var otherUser in otherUsers)
            {
                await InsertMutualFriend(user, otherUser);
            }

            return otherUsers;
        }

        public async Task<UserModel> InsertUser(string username, string password, string salt)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var newUser = new User
            {
                Username = username,
                Password = password,
                Salt = salt,
                DateCreated = DateTime.Now,
                ShowGameStatusToFriends = true,
                ShowGameServerData = true,
                ShowGameDataOnProfile = true,
                PlaySoundOnNewMessages = true,
                PlaySoundsOnNewMessagesInGame = true,
                ShowTimeStampInChat = false,
                PlaySoundsOnLogOn = true,
                ShowFriendsOfFriends = true,
                ShowOfflineFriends  = true,
                ShowNicknames = true,
                ShowVoiceChatServer = true,
                ShowTyping = true,
                ShowTooltipOnLogOn = true,
                ShowTooltipOnDownload = true,
                PlaySoundInChatrooms = true,
                PlaySoundOnVoicecalls = true,
                PlaySoundOnScreenshots = true
            };

            await databaseContext.Set<User>().AddAsync(newUser);

            await databaseContext.SaveChanges();

            return new UserModel
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Password = newUser.Password,
                Nickname = newUser.Nickname,
                ShowGameStatusToFriends = newUser.ShowGameStatusToFriends,
                ShowGameServerData = newUser.ShowGameServerData,
                ShowGameDataOnProfile = newUser.ShowGameDataOnProfile,
                PlaySoundOnNewMessages = newUser.PlaySoundOnNewMessages,
                PlaySoundsOnNewMessagesInGame = newUser.PlaySoundsOnNewMessagesInGame,
                ShowTimeStampInChat = newUser.ShowTimeStampInChat,
                PlaySoundsOnLogOn = newUser.PlaySoundsOnLogOn,
                ShowFriendsOfFriends = newUser.ShowFriendsOfFriends,
                ShowNicknames = newUser.ShowNicknames,
                ShowOfflineFriends = newUser.ShowOfflineFriends,
                ShowVoiceChatServer = newUser.ShowVoiceChatServer,
                ShowTyping = newUser.ShowTyping,
                ShowTooltipOnLogOn = newUser.ShowTooltipOnLogOn,
                ShowTooltipOnDownload = newUser.ShowTooltipOnDownload,
                PlaySoundInChatrooms = newUser.PlaySoundInChatrooms,
                PlaySoundOnVoicecalls = newUser.PlaySoundOnVoicecalls,
                PlaySoundOnScreenshots = newUser.PlaySoundOnScreenshots
            };
        }

        public async Task InsertMutualFriend(UserModel me, UserModel them)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();
            var friends = databaseContext.Set<Friend>();

            var myFriend = await friends.FindAsync(me.Id, them.Id);
            if (myFriend == null)
            {
                await friends.AddAsync(new Friend
                {
                    MeId = me.Id,
                    ThemId = them.Id,
                    Pending = false,
                    DateCreated = DateTime.Now
                });
            }
            else
            {
                myFriend.Message = null;
                myFriend.Pending = false;
                myFriend.DateModified = DateTime.Now;
            }

            var friendOf = await friends.FindAsync(them.Id, me.Id);
            if (friendOf == null)
            {
                await friends.AddAsync(new Friend
                {
                    MeId = them.Id,
                    ThemId = me.Id,
                    Pending = false,
                    DateCreated = DateTime.Now
                });
            }
            else
            {
                friendOf.Message = null;
                friendOf.Pending = false;
                friendOf.DateModified = DateTime.Now;
            }

            await databaseContext.SaveChanges();
        }

        public async Task InsertFriendRequest(UserModel me, UserModel them, string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var friendRequest = new Friend
            {
                MeId = me.Id,
                ThemId = them.Id,
                Pending = true,
                DateCreated = DateTime.Now,
                Message = message
            };

            await databaseContext.Set<Friend>().AddAsync(friendRequest);

            await databaseContext.SaveChanges();
        }

        public async Task<UserModel> QueryUser(string username)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(a => a.Username == username)
                                        .Select(x => new UserModel
                                        {
                                            Id = x.Id,
                                            Username = x.Username,
                                            Password = x.Password,
                                            Nickname = x.Nickname,
                                            ShowGameStatusToFriends = x.ShowGameStatusToFriends,
                                            ShowGameServerData = x.ShowGameServerData,
                                            ShowGameDataOnProfile = x.ShowGameDataOnProfile,
                                            PlaySoundOnNewMessages = x.PlaySoundOnNewMessages,
                                            PlaySoundsOnNewMessagesInGame = x.PlaySoundsOnNewMessagesInGame,
                                            ShowTimeStampInChat = x.ShowTimeStampInChat,
                                            PlaySoundsOnLogOn = x.PlaySoundsOnLogOn,
                                            ShowOfflineFriends = x.ShowOfflineFriends,
                                            ShowFriendsOfFriends = x.ShowFriendsOfFriends,
                                            ShowNicknames = x.ShowNicknames,
                                            ShowVoiceChatServer = x.ShowVoiceChatServer,
                                            ShowTyping = x.ShowTyping,
                                            ShowTooltipOnLogOn = x.ShowTooltipOnLogOn,
                                            ShowTooltipOnDownload = x.ShowTooltipOnDownload,
                                            PlaySoundInChatrooms = x.PlaySoundInChatrooms,
                                            PlaySoundOnVoicecalls = x.PlaySoundOnVoicecalls,
                                            PlaySoundOnScreenshots = x.PlaySoundOnScreenshots
                                        })
                                        .FirstOrDefaultAsync();
        }

        public async Task<UserModel> QueryUser(int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(a => a.Id == userId)
                                        .Select(x => new UserModel
                                        {
                                            Id = x.Id,
                                            Username = x.Username,
                                            Password = x.Password,
                                            Nickname = x.Nickname
                                        })
                                        .FirstOrDefaultAsync();
        }

        public async Task<List<UserModel>> QueryUsers(string username)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(a => a.Username.Contains(username))
                                        .Select(x => new UserModel
                                        {
                                            Id = x.Id,
                                            Username = x.Username,
                                            Nickname = x.Nickname
                                        })
                                        .ToListAsync();
        }

        public async Task<List<UserModel>> QueryFriends(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(a => a.Id == user.Id)
                                        .SelectMany(a => a.MyFriends)
                                        .Where(x => x.Pending == false)
                                        .Select(x => new UserModel
                                        {
                                            Id = x.Them.Id,
                                            Username = x.Them.Username,
                                            Nickname = x.Them.Nickname
                                        })
                                        .ToListAsync();
        }

        public async Task<List<UserModel>> QueryFriendsOfFriends(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(a => a.Id == user.Id)
                                        .SelectMany(a => a.MyFriends.SelectMany(b => b.Them.MyFriends))
                                        .Where(x => x.Pending == false)
                                        .Select(x => new UserModel
                                        {
                                            Id = x.Them.Id,
                                            Username = x.Them.Username,
                                            Nickname = x.Them.Nickname
                                        })
                                        .ToListAsync();
        }

        public async Task<List<FriendRequestModel>> QueryPendingFriendRequestsSelf(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(a => a.Id == user.Id)
                                        .SelectMany(x => x.MyFriends)
                                        .Where(x => x.Pending)
                                        .Select(x => new FriendRequestModel
                                        {
                                            Id = x.Them.Id,
                                            Username = x.Them.Username,
                                            Nickname = x.Them.Nickname,
                                            Message = x.Message
                                        })
                                        .ToListAsync();
        }

        public async Task<List<FriendRequestModel>> QueryPendingFriendRequests(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(a => a.Id == user.Id)
                                        .SelectMany(x => x.FriendsOf)
                                        .Where(x => x.Pending)
                                        .Select(x => new FriendRequestModel
                                        {
                                            Id = x.Me.Id,
                                            Username = x.Me.Username,
                                            Nickname = x.Me.Nickname,
                                            Message = x.Message
                                        })
                                        .ToListAsync();
        }

        public async Task DeletePendingFriendRequest(UserModel me, params FriendRequestModel[] thems)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var friends = databaseContext.Set<Friend>();
            foreach (var them in thems)
            {
                var myFriend = await friends.FindAsync(me.Id, them.Id);
                if (myFriend != null)
                {
                    friends.Remove(myFriend);
                }

                var friendOf = await friends.FindAsync(them.Id, me.Id);
                if (friendOf != null)
                {
                    friends.Remove(friendOf);
                }
            }

            await databaseContext.SaveChanges();
        }

        public async Task UpdateNickname(UserModel user, string nickname)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var entity = await databaseContext.Set<User>().FindAsync(user.Id);
            if (entity == null)
            {
                return;
            }

            entity.Nickname = nickname;

            await databaseContext.SaveChanges();
        }

        public async Task RemoveFriend(UserModel me, UserModel them)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var friends = databaseContext.Set<Friend>();

            var myFriend = await friends.FindAsync(me.Id, them.Id);
            if (myFriend != null)
            {
                friends.Remove(myFriend);
            }

            var friendOf = await friends.FindAsync(them.Id, me.Id);
            if (friendOf != null)
            {
                friends.Remove(friendOf);
            }

            await databaseContext.SaveChanges();
        }

        public async Task SaveUserPrefs(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var entity = await databaseContext.Set<User>().FindAsync(user.Id);
            if (entity == null)
            {
                return;
            }

            // Update all preferences
            entity.ShowGameStatusToFriends = user.ShowGameStatusToFriends;
            entity.ShowGameServerData = user.ShowGameServerData;
            entity.ShowGameDataOnProfile = user.ShowGameDataOnProfile;
            entity.PlaySoundOnNewMessages = user.PlaySoundOnNewMessages;
            entity.PlaySoundsOnNewMessagesInGame = user.PlaySoundsOnNewMessagesInGame;
            entity.ShowTimeStampInChat = user.ShowTimeStampInChat;
            entity.PlaySoundsOnLogOn = user.PlaySoundsOnLogOn;
            entity.ShowFriendsOfFriends = user.ShowFriendsOfFriends;
            entity.ShowOfflineFriends = user.ShowOfflineFriends;
            entity.ShowNicknames = user.ShowNicknames;
            entity.ShowVoiceChatServer = user.ShowVoiceChatServer;
            entity.ShowTyping = user.ShowTyping;
            entity.ShowTooltipOnLogOn = user.ShowTooltipOnLogOn;
            entity.ShowTooltipOnDownload = user.ShowTooltipOnDownload;
            entity.PlaySoundInChatrooms = user.PlaySoundInChatrooms;
            entity.PlaySoundOnVoicecalls = user.PlaySoundOnVoicecalls;
            entity.PlaySoundOnScreenshots = user.PlaySoundOnScreenshots;

            await databaseContext.SaveChanges();
        }

        public async Task<UserGroup> CreateUserGroup(UserModel user, string groupName)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var newGroup = new UserGroup
            {
                Name = groupName,
                OwnerId = user.Id,
                MemberIds = []
            };

            await databaseContext.Set<UserGroup>().AddAsync(newGroup);

            await databaseContext.SaveChanges();

            return newGroup;
        }

        public async Task<UserGroup> GetUserGroup(int groupId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<UserGroup>()
                                        .AsNoTracking()
                                        .Where(a => a.Id.Equals(groupId))
                                        .Select(x => new UserGroup
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            OwnerId = x.OwnerId,
                                            MemberIds = x.MemberIds
                                        })
                                        .FirstOrDefaultAsync();
        }

        public async Task<List<UserGroup>> GetAllUserGroups(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<UserGroup>()
                                        .AsNoTracking()
                                        .Where(ug => ug.OwnerId == user.Id)
                                        .Select(x => new UserGroup
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            OwnerId = x.OwnerId,
                                            MemberIds = x.MemberIds
                                        })
                                        .ToListAsync();
        }

        public async Task RemoveUserGroup(int groupId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var entity = await databaseContext.Set<UserGroup>().FindAsync(groupId);
            if (entity == null)
            {
                return;
            }

            databaseContext.Set<UserGroup>().Remove(entity);

            await databaseContext.SaveChanges();
        }

        public async Task<List<int>> GetUserGroupMembers(int groupId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var memberIds = await databaseContext.Set<UserGroup>()
                .AsNoTracking()
                .Where(ug => ug.Id == groupId)
                .Select(ug => ug.MemberIds)
                .FirstOrDefaultAsync();

            return memberIds;
        }

        public async Task RenameUserGroup(int groupId, string newName)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var entity = await databaseContext.Set<UserGroup>().FindAsync(groupId);
            if (entity == null)
            {
                return;
            }

            entity.Name = newName;

            await databaseContext.SaveChanges();
        }

        public async Task AddMemberToUserGroup(int groupId, int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var entity = await databaseContext.Set<UserGroup>().FindAsync(groupId);
            if (entity == null)
            {
                return;
            }

            entity.MemberIds.Add(userId);

            await databaseContext.SaveChanges();
        }

        public async Task RemoveMemberFromUserGroup(int groupId, int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var entity = await databaseContext.Set<UserGroup>().FindAsync(groupId);
            if (entity == null)
            {
                return;
            }

            entity.MemberIds.Remove(userId);

            await databaseContext.SaveChanges();
        }

        public async Task<UserGroup> GetUserGroup(string groupName)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<UserGroup>()
                                        .AsNoTracking()
                                        .Where(a => a.Name.Equals(groupName))
                                        .Select(x => new UserGroup
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            OwnerId = x.OwnerId,
                                            MemberIds = x.MemberIds
                                        })
                                        .FirstOrDefaultAsync();
        }

        public async Task<List<UserServerList>> GetAllUserFavoriteServers(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<UserServerList>()
                                        .AsNoTracking()
                                        .Where(ug => ug.UserId == user.Id)
                                        .Select(x => new UserServerList
                                        {
                                            Id = x.Id,
                                            UserId = x.UserId,
                                            GameId = x.GameId,
                                            GameIp = x.GameIp,
                                            GamePort = x.GamePort
                                        })
                                        .ToListAsync();
        }

        public async Task AddUserFavoriteServer(int gameId, int gameIp, int gamePort, int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var newFavorite = new UserServerList
            {
                UserId = userId,
                GameId = gameId,
                GameIp = gameIp,
                GamePort = gamePort
            };

            await databaseContext.Set<UserServerList>().AddAsync(newFavorite);

            await databaseContext.SaveChanges();
        }

        public async Task RemoveUserFavoriteServer(int gameId, int gameIp, int gamePort, int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var entity = await databaseContext.Set<UserServerList>()
                             .FirstOrDefaultAsync(usl => usl.UserId == userId
                             && usl.GameId == gameId
                             && usl.GameIp == gameIp
                             && usl.GamePort == gamePort);

            if (entity == null)
            {
                return;
            }

            databaseContext.Set<UserServerList>().Remove(entity);

            await databaseContext.SaveChanges();
        }

        public async Task<ChatroomModel> InsertChatroom(ChatroomModel chatroom)
        {

            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var newChat = new ChatroomEnt
            {
                Cid = chatroom.Id,
                Name = chatroom.Name,
                MOTD = chatroom.MOTD,
                Visibility = chatroom.Visibility,
                Administrators = chatroom.Administrators,
                Moderators = chatroom.Moderators,
                PowerUsers = chatroom.PowerUsers,
                Users = chatroom.Users,
                SilencedUsers = chatroom.SilencedUsers,
                ShowJoinLeaveMessages = chatroom.ShowJoinLeaveMessages,
                DefaultPerms = chatroom.DefaultPerms,
                SavedRoom = chatroom.SavedRoom,
                Password = chatroom.Password
            };

            await databaseContext.Set<ChatroomEnt>().AddAsync(newChat);

            await databaseContext.SaveChanges();

            return new ChatroomModel
            {
                Id = newChat.Cid,
                Name = newChat.Name,
                MOTD = newChat.MOTD,
                Visibility = newChat.Visibility,
                Administrators = newChat.Administrators,
                Moderators = newChat.Moderators,
                PowerUsers = newChat.PowerUsers,
                Users = newChat.Users,
                SilencedUsers = newChat.SilencedUsers,
                ShowJoinLeaveMessages = newChat.ShowJoinLeaveMessages,
                DefaultPerms = newChat.DefaultPerms,
                SavedRoom = newChat.SavedRoom,
                Password = newChat.Password
            };

        }

        public async Task<List<ChatroomModel>> GetAllChatrooms()
        {
        using var scope = _serviceProvider.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

        return await databaseContext.Set<ChatroomEnt>()
                                    .AsNoTracking()
                                    .Select(x => new ChatroomModel
                                    {
                                        Id = x.Cid,
                                        Name = x.Name,
                                        MOTD = x.MOTD,
                                        Visibility = x.Visibility,
                                        Administrators = x.Administrators,
                                        Moderators = x.Moderators,
                                        PowerUsers = x.PowerUsers,
                                        Users = x.Users,
                                        SilencedUsers = x.SilencedUsers,
                                        ShowJoinLeaveMessages = x.ShowJoinLeaveMessages,
                                        DefaultPerms = x.DefaultPerms,
                                        SavedRoom = x.SavedRoom,
                                        Password = x.Password
                                    })
                                    .ToListAsync();
        }

        public async Task<List<ChatroomModel>> GetAllVisibleChatrooms()
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<ChatroomEnt>()
                                        .AsNoTracking()
                                        .Where(x => x.Visibility == 1)
                                        .Select(x => new ChatroomModel
                                        {
                                            Id = x.Cid,
                                            Name = x.Name,
                                            MOTD = x.MOTD,
                                            Visibility = x.Visibility,
                                            Administrators = x.Administrators,
                                            Moderators = x.Moderators,
                                            PowerUsers = x.PowerUsers,
                                            Users = x.Users,
                                            SilencedUsers = x.SilencedUsers,
                                            ShowJoinLeaveMessages = x.ShowJoinLeaveMessages,
                                            DefaultPerms = x.DefaultPerms,
                                            SavedRoom = x.SavedRoom,
                                            Password = x.Password
                                        })
                                        .ToListAsync();
        }

        public async Task<ChatroomModel> QueryChatroom(byte[] cid)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<ChatroomEnt>()
                                        .AsNoTracking()
                                        .Where(a => a.Cid.Equals(cid))
                                        .Select(x => new ChatroomModel
                                        {
                                            Id = x.Cid,
                                            Name = x.Name,
                                            MOTD = x.MOTD,
                                            Visibility = x.Visibility,
                                            Administrators = x.Administrators,
                                            Moderators = x.Moderators,
                                            PowerUsers = x.PowerUsers,
                                            Users = x.Users,
                                            SilencedUsers = x.SilencedUsers,
                                            ShowJoinLeaveMessages = x.ShowJoinLeaveMessages,
                                            DefaultPerms = x.DefaultPerms,
                                            SavedRoom = x.SavedRoom,
                                            Silenced = x.Silenced,
                                            Password = x.Password,
                                            GameLobbyPlayers = x.GameLobbyPlayers,
                                            GameLobbyHost = x.GameLobbyHost,
                                            GameLobbyID = x.GameLobbyID,
                                            GameLobbyIP = x.GameLobbyIP,
                                            GameLobbyPort = x.GameLobbyPort
                                        })
                                        .FirstOrDefaultAsync();
        }

        public async Task<ChatroomModel> QueryChatroom(string topic)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<ChatroomEnt>()
                                        .AsNoTracking()
                                        .Where(a => a.Name == topic)
                                        .Select(x => new ChatroomModel
                                        {
                                            Id = x.Cid,
                                            Name = x.Name,
                                            MOTD = x.MOTD,
                                            Visibility = x.Visibility,
                                            Administrators = x.Administrators,
                                            Moderators = x.Moderators,
                                            PowerUsers = x.PowerUsers,
                                            Users = x.Users,
                                            SilencedUsers = x.SilencedUsers,
                                            ShowJoinLeaveMessages = x.ShowJoinLeaveMessages,
                                            DefaultPerms = x.DefaultPerms,
                                            SavedRoom = x.SavedRoom,
                                            Silenced = x.Silenced,
                                            Password = x.Password,
                                            GameLobbyPlayers = x.GameLobbyPlayers,
                                            GameLobbyHost = x.GameLobbyHost,
                                            GameLobbyID = x.GameLobbyID,
                                            GameLobbyIP = x.GameLobbyIP,
                                            GameLobbyPort = x.GameLobbyPort
                                        })
                                        .FirstOrDefaultAsync();
        }

        public async Task<List<int>> QueryCurrentChatroomUsers(byte[] cid)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<ChatroomEnt>()
                                        .AsNoTracking()
                                        .Where(a => a.Cid.Equals(cid))
                                        .Select(x => x.Users)
                                        .FirstOrDefaultAsync();
        }

        public async Task<List<UserModel>> QueryUsers(List<int> userIds)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            return await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(u => userIds.Contains(u.Id))
                                        .Select(x => new UserModel
                                        {
                                            Id = x.Id,
                                            Username = x.Username,
                                            Nickname = x.Nickname
                                        })
                                        .ToListAsync();
        }

        public async Task<string> GetChatroomName(byte[] cid)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chat = await databaseContext.Set<ChatroomEnt>()
                                        .AsNoTracking()
                                        .Where(a => a.Cid.Equals(cid))
                                        .FirstOrDefaultAsync();

            if (chat == null)
            {
                return "";
            }
                
            return chat.Name;
        }

        public async Task<int> GetUserHighestChatPowerLevel(byte[] roomId, int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Cid == roomId);

            if (chatroom != null)
            {
                // Check if the user is an admin
                if (chatroom.Administrators.Contains(userId))
                    return 5;

                // Check if the user is a moderator
                if (chatroom.Moderators.Contains(userId))
                    return 4;

                // Check if the user is a power user
                if (chatroom.PowerUsers.Contains(userId))
                    return 3;

                // Check if the user is silenced
                if (chatroom.SilencedUsers.Contains(userId))
                    return 1;
            }

            // If the user is not found in any list just a normal user.
            return 2;
        }

        public async Task<List<(int UserId, int HighestLevel)>> GetUsersHighestChatPowerLevels(byte[] roomId, List<int> userIds)
        {
            var highestLevels = new List<(int UserId, int HighestLevel)>();

            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Cid == roomId);

            if (chatroom != null)
            {
                foreach (var userId in userIds)
                {
                    var highestLevel = chatroom.Administrators.Contains(userId) ? 5 :
                                       chatroom.Moderators.Contains(userId) ? 4 :
                                       chatroom.PowerUsers.Contains(userId) ? 3 :
                                       chatroom.SilencedUsers.Contains(userId) ? 1 : 2;
                    highestLevels.Add((userId, highestLevel));
                }
            }

            return highestLevels;
        }

        public async Task<List<byte[]>> GetChatrooms()
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatSids = await databaseContext.Set<ChatroomEnt>()
                .AsNoTracking()
                .Select(ug => ug.Cid)
                .ToListAsync();

            return chatSids;
        }

        public async Task UserEnteredRoom(byte[] roomId, int userid)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                        .Where(a => a.Cid.Equals(roomId))
                                        .FirstOrDefaultAsync();

            if (chatroom == null)
            {
                return;
            }

            if (!chatroom.Users.Contains(userid))
            {
                chatroom.Users.Add(userid);
            }

            await databaseContext.SaveChanges();
        }

        public async Task<int> UserLeftRoom(byte[] roomId, int userid)
        {
            int roomDeleted = 0;

            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            if (chatroom == null || chatroom.Users == null)
            {
                return roomDeleted;
            }

            chatroom.Users.Remove(userid);

            if (chatroom.SavedRoom == 0)
            {
                // Check if Users collection is empty after removing the user
                if (chatroom.Users.Count == 0)
                {
                    // If the Users collection is empty, remove the chatroom entirely
                    databaseContext.Set<ChatroomEnt>().Remove(chatroom);
                    roomDeleted = 1;
                }
            }

            await databaseContext.SaveChanges();

            return roomDeleted;
        }

        public async Task UpdateChatroomName(byte[] roomId, string name)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                        .Where(a => a.Cid.Equals(roomId))
                                        .FirstOrDefaultAsync();

            if (chatroom == null)
            {
                return;
            }

            chatroom.Name = name;

            await databaseContext.SaveChanges();
        }

        public async Task UpdateChatroomUserPerms(byte[] roomId, int userId, int perms)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            chatroom.Administrators.Remove(userId);
            chatroom.Moderators.Remove(userId);
            chatroom.PowerUsers.Remove(userId);
            chatroom.SilencedUsers.Remove(userId);

            switch (perms)
            {
                case 1: chatroom.SilencedUsers.Add(userId); break;
                case 3: chatroom.PowerUsers.Add(userId); break;
                case 4: chatroom.Moderators.Add(userId); break;
                case 5: chatroom.Administrators.Add(userId); break;
            }

            await databaseContext.SaveChanges();
        }

        public async Task UpdateChatroomSaveStatus(byte[] roomId, byte saveToggle)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            if (saveToggle > 1)
            {
                saveToggle = 1;
            }

            chatroom.SavedRoom = saveToggle;

            await databaseContext.SaveChanges();
        }

        public async Task UpdateChatroomMOTD(byte[] roomId, string motd)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                        .Where(a => a.Cid.Equals(roomId))
                                        .FirstOrDefaultAsync();

            if (chatroom == null)
            {
                return;
            }

            chatroom.MOTD = motd;

            await databaseContext.SaveChanges();
        }

        public async Task UpdateChatroomSilenceStatus(byte[] roomId, byte silenceToggle)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            if (silenceToggle > 1)
            {
                silenceToggle = 1;
            }

            chatroom.Silenced = silenceToggle;

            await databaseContext.SaveChanges();
        }

        public async Task UpdateChatroomJoinNotification(byte[] roomId, byte notificationToggle)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            if (notificationToggle > 1)
            {
                notificationToggle = 1;
            }

            chatroom.ShowJoinLeaveMessages = notificationToggle;

            await databaseContext.SaveChanges();
        }

        public async Task UpdateChatroomVisibility(byte[] roomId, int visibility)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            if (visibility > 2 || visibility < 1)
            {
                visibility = 1;
            }

            if (visibility == 1 && chatroom.Password.Length > 0)
            {
                chatroom.Password = "";
            }


            chatroom.Visibility = visibility;

            await databaseContext.SaveChanges();
        }

        public async Task UpdateChatroomPassword(byte[] roomId, string password)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            chatroom.Password = password;

            await databaseContext.SaveChanges();
        }

        public async Task KickChatroomUser(byte[] roomId, int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            chatroom.Users.Remove(userId);

            await databaseContext.SaveChanges();
        }

        public async Task UpdateChatroomDefaultPerms(byte[] roomId, int perms)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            if (perms > 5)
            {
                perms = 2;
            }

            chatroom.DefaultPerms = perms;

            await databaseContext.SaveChanges();
        }

        public async Task<bool> IsRoomSilenced(byte[] roomId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(roomId));

            return chatroom.Silenced > 0;
        }

        public async Task<List<UserModel>> QueryUsersInChat(byte[] Cid)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            // Fetch user data for users in the specified chatroom
            var usersInChatroom = await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(user => databaseContext.Set<ChatroomEnt>()
                                                                .Any(chatroom => chatroom.Cid.SequenceEqual(Cid) && chatroom.Users.Contains(user.Id)))
                                        .Select(user => new UserModel
                                        {
                                            Id = user.Id,
                                            Username = user.Username,
                                            Nickname = user.Nickname
                                        })
                                        .ToListAsync();

            return usersInChatroom;
        }

        public async Task<List<UserModel>> QueryGameLobbyUsersInChat(byte[] Cid)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            // Fetch user data for users in the specified chatroom
            var usersInChatroom = await databaseContext.Set<User>()
                                        .AsNoTracking()
                                        .Where(user => databaseContext.Set<ChatroomEnt>()
                                                                .Any(chatroom => chatroom.Cid.SequenceEqual(Cid) && chatroom.GameLobbyPlayers.Contains(user.Id)))
                                        .Select(user => new UserModel
                                        {
                                            Id = user.Id,
                                            Username = user.Username,
                                            Nickname = user.Nickname
                                        })
                                        .ToListAsync();

            return usersInChatroom;
        }

        public async Task StartChatroomGameLobby(byte[] cid, int gameId, int gameIp, int gamePort, List<int> lobbyUsers)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(cid));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            if (gameId == 0)
            {
                //Admin/Mod Stopped hosting. Wipe all data.
                chatroom.GameLobbyID = 0;
                chatroom.GameLobbyIP = 0;
                chatroom.GameLobbyPort = 0;
                chatroom.GameLobbyPlayers = [];
            }
            else
            {
                chatroom.GameLobbyID = gameId;
                chatroom.GameLobbyIP = gameIp;
                chatroom.GameLobbyPort = gamePort;
                chatroom.GameLobbyPlayers = lobbyUsers;
            }

            await databaseContext.SaveChanges();
        }

        public async Task UserJoinedGameLobby(byte[] cid, int userid)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(cid));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            if (!chatroom.GameLobbyPlayers.Contains(userid))
            {
                chatroom.GameLobbyPlayers.Add(userid);
            }

            await databaseContext.SaveChanges();
        }

        public async Task UserLeftGameLobby(byte[] cid, int userid)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var chatroom = await databaseContext.Set<ChatroomEnt>()
                                                .FirstOrDefaultAsync(a => a.Cid.Equals(cid));

            if (chatroom == null || chatroom.Users == null)
            {
                return;
            }

            chatroom.GameLobbyPlayers.Remove(userid);

            await databaseContext.SaveChanges();
        }
    }
}
