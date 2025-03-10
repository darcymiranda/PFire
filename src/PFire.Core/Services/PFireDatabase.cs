﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PFire.Core.Models;
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
        Task<UserModel> QueryUser(uint userId);
        Task<List<UserModel>> QueryUsers(string username);
        Task<List<UserModel>> QueryFriends(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequestsSelf(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequests(UserModel otherUser);
        Task DeletePendingFriendRequest(UserModel me, params FriendRequestModel[] thems);
        Task UpdateNickname(UserModel user, string nickname);
        Task RemoveFriend(UserModel me, UserModel them);
        Task<GroupModel> CreateGroup(UserModel user, string name);
        Task<List<GroupModel>> GetGroupsByOwner(int ownerId);
        Task RenameGroup(int ownerId, int groupId, string name);
        Task RemoveGroup(int ownerId, int groupId);
        Task<ClientPreferencesModel> GetClientPreferences(UserModel user);
        Task SaveClientPreferences(UserModel user);
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
                Salt = salt
            };

            await databaseContext.Set<User>().AddAsync(newUser);

            await databaseContext.SaveChanges();

            return new UserModel
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Password = newUser.Password,
                Nickname = newUser.Nickname
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
                });
            }
            else
            {
                myFriend.Message = null;
                myFriend.Pending = false;
            }

            var friendOf = await friends.FindAsync(them.Id, me.Id);
            if (friendOf == null)
            {
                await friends.AddAsync(new Friend
                {
                    MeId = them.Id,
                    ThemId = me.Id,
                    Pending = false,
                });
            }
            else
            {
                friendOf.Message = null;
                friendOf.Pending = false;
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
                                            Nickname = x.Nickname
                                        })
                                        .FirstOrDefaultAsync();
        }

        public async Task<UserModel> QueryUser(uint userId)
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

        public async Task<GroupModel> CreateGroup(UserModel user, string name)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var newGroup = new Group
            {
                Name = name,
                OwnerId = user.Id,
            };

            await databaseContext.Set<Group>().AddAsync(newGroup);
            await databaseContext.SaveChanges();

            return new GroupModel
            {
                Id = newGroup.Id,
                Name = name
            };
        }

        public async Task<List<GroupModel>> GetGroupsByOwner(int ownerId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var groups = await databaseContext.Set<Group>()
                .Where(g => g.OwnerId == ownerId)
                .ToListAsync();

            var groupModels = groups.Select(groups => new GroupModel
            {
                Id = groups.Id,
                Name = groups.Name
            }).ToList();

            return groupModels;
        }

        public async Task RenameGroup(int ownerId, int groupId, string name)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var group = await databaseContext.Set<Group>().FindAsync(groupId);

            if (group is not null
                && group.OwnerId == ownerId)
            {
                group.Name = name;
                await databaseContext.SaveChanges();
            }
        }

        public async Task RemoveGroup(int ownerId, int groupId)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var group = await databaseContext.Set<Group>().FindAsync(groupId);

            if (group is not null
                && group.OwnerId == ownerId)
            {
                databaseContext.Set<Group>().Remove(group);
                await databaseContext.SaveChanges();
            }
        }

        public async Task<ClientPreferencesModel> GetClientPreferences(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var clientPreferences = await databaseContext.Set<ClientPreferences>()
                .AsNoTracking()
                .Where(c => c.UserId == user.Id)
                .Select(c => new ClientPreferencesModel
                {
                    ChatShowTimestamps = c.ChatShowTimestamps,
                    GameStatusShowFriendOfFriends = c.GameStatusShowFriendOfFriends,
                    GameStatusShowMyFriends = c.GameStatusShowMyFriends,
                    GameStatusShowMyGameServer = c.GameStatusShowMyGameServer,
                    GameStatusShowMyProfile = c.GameStatusShowMyProfile,
                    NotificationConnectionStateChanges = c.NotificationConnectionStateChanges,
                    NotificationDownloadStartsFinishes = c.NotificationDownloadStartsFinishes,
                    NotificationFriendLogsOnOff = c.NotificationFriendLogsOnOff,
                    PlaySoundFriendLogsOnOff = c.PlaySoundFriendLogsOnOff,
                    PlaySoundReceiveMessageWhileGaming = c.PlaySoundReceiveMessageWhileGaming,
                    PlaySoundScreenshotWhileGaming = c.PlaySoundScreenshotWhileGaming,
                    PlaySoundSendOrReceiveMessage = c.PlaySoundSendOrReceiveMessage,
                    PlaySoundSendReceiveVoiceChatRequest = c.PlaySoundSendReceiveVoiceChatRequest,
                    PlaySoundSomeoneJoinsLeaveChatroom = c.PlaySoundSomeoneJoinsLeaveChatroom,
                    ShowNicknames = c.ShowNicknames,
                    ShowOfflineFriends = c.ShowOfflineFriends,
                    ShowVoiceChatServerToFriends = c.ShowVoiceChatServerToFriends,
                    ShowWhenTyping = c.ShowWhenTyping
                })
                .FirstOrDefaultAsync();

            if(clientPreferences is null)
            {
                //Set defaults if we want here
                clientPreferences = new ClientPreferencesModel();
            }

            return clientPreferences;
        }

        public async Task SaveClientPreferences(UserModel user)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var clientPreferences = await databaseContext.Set<ClientPreferences>().FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (clientPreferences is null)
            {
                clientPreferences = new ClientPreferences { UserId = user.Id };
                await databaseContext.Set<ClientPreferences>().AddAsync(clientPreferences);
            }

            clientPreferences.GameStatusShowMyFriends = user.ClientPreferences.GameStatusShowMyFriends;
            clientPreferences.GameStatusShowMyGameServer = user.ClientPreferences.GameStatusShowMyGameServer;
            clientPreferences.GameStatusShowMyProfile = user.ClientPreferences.GameStatusShowMyProfile;
            clientPreferences.PlaySoundSendOrReceiveMessage = user.ClientPreferences.PlaySoundSendOrReceiveMessage;
            clientPreferences.PlaySoundReceiveMessageWhileGaming = user.ClientPreferences.PlaySoundReceiveMessageWhileGaming;
            clientPreferences.ChatShowTimestamps = user.ClientPreferences.ChatShowTimestamps;
            clientPreferences.PlaySoundFriendLogsOnOff = user.ClientPreferences.PlaySoundFriendLogsOnOff;
            clientPreferences.GameStatusShowFriendOfFriends = user.ClientPreferences.GameStatusShowFriendOfFriends;
            clientPreferences.ShowOfflineFriends = user.ClientPreferences.ShowOfflineFriends;
            clientPreferences.ShowNicknames = user.ClientPreferences.ShowNicknames;
            clientPreferences.ShowVoiceChatServerToFriends = user.ClientPreferences.ShowVoiceChatServerToFriends;
            clientPreferences.ShowWhenTyping = user.ClientPreferences.ShowWhenTyping;
            clientPreferences.NotificationFriendLogsOnOff = user.ClientPreferences.NotificationFriendLogsOnOff;
            clientPreferences.NotificationDownloadStartsFinishes = user.ClientPreferences.NotificationDownloadStartsFinishes;
            clientPreferences.PlaySoundSomeoneJoinsLeaveChatroom = user.ClientPreferences.PlaySoundSomeoneJoinsLeaveChatroom;
            clientPreferences.PlaySoundSendReceiveVoiceChatRequest = user.ClientPreferences.PlaySoundSendReceiveVoiceChatRequest;
            clientPreferences.PlaySoundScreenshotWhileGaming = user.ClientPreferences.PlaySoundScreenshotWhileGaming;
            clientPreferences.NotificationConnectionStateChanges = user.ClientPreferences.NotificationConnectionStateChanges;

            await databaseContext.SaveChanges();
        }
    }
}
