using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PFire.Infrastructure.Entities;
using PFire.Infrastructure.Models;

namespace PFire.Infrastructure.Services
{
    public interface IPFireDatabase
    {
        Task<UserModel> InsertUser(string username, string password, string salt);
        Task InsertMutualFriend(UserModel user1, UserModel user2);
        Task InsertFriendRequest(UserModel owner, UserModel requestedUsername, string message);
        Task<UserModel> QueryUser(string username);
        Task<List<UserModel>> QueryUsers(string username);
        Task<List<UserModel>> QueryFriends(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequestsSelf(UserModel user);
        Task<List<FriendRequestModel>> QueryPendingFriendRequests(UserModel otherUser);
        Task DeletePendingFriendRequest(UserModel me, params FriendRequestModel[] pendingFriendRequestIds);
        Task UpdateNickname(UserModel user, string nickname);
    }

    internal class PFireDatabase : IPFireDatabase
    {
        private readonly IServiceProvider _serviceProvider;

        public PFireDatabase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

            databaseContext.Set<User>().Add(newUser);
            await databaseContext.SaveChangesAsync();

            //TODO: is model updated with new id?
            return new UserModel
            {
                Id = newUser.Id,
                Username = newUser.Username,
                Nickname = newUser.Nickname
            };
        }

        public async Task InsertMutualFriend(UserModel user1, UserModel user2)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();
            var friends = databaseContext.Set<Friend>();

            var entity1 = new Friend
            {
                MeId = user1.Id,
                ThemId = user2.Id
            };

            var entity2 = new Friend
            {
                MeId = user2.Id,
                ThemId = user1.Id
            };

            friends.Add(entity1);
            friends.Add(entity2);

            await databaseContext.SaveChangesAsync();
        }

        public async Task InsertFriendRequest(UserModel owner, UserModel requestedUsername, string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var pendingFriendRequest = new Friend
            {
                MeId = owner.Id,
                ThemId = requestedUsername.Id,
                Message = message,
                Pending = true
            };

            databaseContext.Set<Friend>().Add(pendingFriendRequest);

            await databaseContext.SaveChangesAsync();
        }

        public async Task<UserModel> QueryUser(string username)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
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

        public async Task<List<UserModel>> QueryUsers(string username)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
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
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
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
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
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
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            //TODO: use Model instead of Entity
            return await reader.Query<User>()
                               .Where(a => a.Id == user.Id)
                               .SelectMany(x => x.FriendsOf)
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

        public async Task DeletePendingFriendRequest(UserModel me, params FriendRequestModel[] pendingFriendRequestIds)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();
            var pendingFriendRequests = databaseContext.Set<Friend>();

            foreach (var pendingFriendRequest in pendingFriendRequestIds)
            {
                var pendingFriendRequest1 = await pendingFriendRequests.FindAsync(me.Id, pendingFriendRequest.Id);
                var pendingFriendRequest2 = await pendingFriendRequests.FindAsync(pendingFriendRequest.Id, me.Id);

                pendingFriendRequests.Remove(pendingFriendRequest1);
                pendingFriendRequests.Remove(pendingFriendRequest2);
            }

            await databaseContext.SaveChangesAsync();
        }

        public async Task UpdateNickname(UserModel user, string nickname)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var userToUpdate = await databaseContext.Set<User>().SingleAsync(x => x.Id == user.Id);
            userToUpdate.Nickname = nickname;

            await databaseContext.SaveChangesAsync();
        }
    }
}
