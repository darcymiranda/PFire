using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        Task<IList<UserModel>> AddEveryoneAsFriends(UserModel user);
        Task<UserModel> InsertUser(string username, string password, string salt);
        Task InsertMutualFriend(UserModel me, UserModel them);
        Task InsertFriendRequest(UserModel me, UserModel them, string message);
        Task<UserModel> QueryUser(string username);
        Task<UserModel> QueryUser(int userId);
        Task<List<UserModel>> QueryUsers(string username);
        Task<List<UserModel>> QueryFriends(UserModel user);
        Task<List<UserModel>> QueryFriendsOfFriends(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequestsSelf(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequests(UserModel otherUser);
        Task DeletePendingFriendRequest(UserModel me, params FriendRequestModel[] thems);
        Task UpdateNickname(UserModel user, string nickname);
        Task RemoveFriend(UserModel me, UserModel them);
        Task SaveUserPrefs(UserModel user);
        Task<UserGroup> CreateUserGroup(UserModel user, string groupName);
        Task<UserGroup> GetUserGroup(int groupId);
        Task<UserGroup> GetUserGroup(string groupName);
        Task<List<UserGroup>> GetAllUserGroups(UserModel user);
        Task RemoveUserGroup(int groupId);
        Task<List<int>> GetUserGroupMembers(int groupId);
        Task RenameUserGroup(int groupId, string newName);
        Task AddMemberToUserGroup(int groupId, int userId);
        Task RemoveMemberFromUserGroup(int groupId, int userId);
        Task<List<UserServerList>> GetAllUserFavoriteServers (UserModel user);
        Task AddUserFavoriteServer (int gameId, int gameIp, int gamePort, int userId);
        Task RemoveUserFavoriteServer (int gameId, int gameIp, int gamePort, int userId);

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

        async Task<UserGroup> IPFireDatabase.CreateUserGroup(UserModel user, string groupName)
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

        async Task<UserGroup> IPFireDatabase.GetUserGroup(int groupId)
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

        async Task<List<UserGroup>> IPFireDatabase.GetAllUserGroups(UserModel user)
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

        async Task IPFireDatabase.RemoveUserGroup(int groupId)
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

        async Task<List<int>> IPFireDatabase.GetUserGroupMembers(int groupId)
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

        async Task IPFireDatabase.RenameUserGroup(int groupId, string newName)
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

        async Task IPFireDatabase.AddMemberToUserGroup(int groupId, int userId)
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

        async Task IPFireDatabase.RemoveMemberFromUserGroup(int groupId, int userId)
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

        async Task<UserGroup> IPFireDatabase.GetUserGroup(string groupName)
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

        async Task<List<UserServerList>> IPFireDatabase.GetAllUserFavoriteServers(UserModel user)
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

        async Task IPFireDatabase.AddUserFavoriteServer(int gameId, int gameIp, int gamePort, int userId)
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

        async Task IPFireDatabase.RemoveUserFavoriteServer(int gameId, int gameIp, int gamePort, int userId)
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
    }
}
