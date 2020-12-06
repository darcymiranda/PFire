using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PFire.Infrastructure.Entities;

namespace PFire.Infrastructure.Services
{
    public interface IPFireDatabase
    {
        Task<User> InsertUser(string username, string password, string salt);
        Task InsertMutualFriend(User user1, User user2);
        Task InsertFriendRequest(User owner, User requestedUsername, string message);
        Task<User> QueryUser(int userId);
        Task<User> QueryUser(string username);
        Task<List<User>> QueryUsers(string username);
        Task<List<User>> QueryFriends(User user);
        Task<List<Friend>> QueryPendingFriendRequestsSelf(User user);
        Task<List<Friend>> QueryPendingFriendRequests(User otherUser);
        Task DeletePendingFriendRequest(params Friend[] pendingFriendRequestIds);
        Task UpdateNickname(User user, string nickname);
    }

    internal class PFireDatabase : IPFireDatabase
    {
        private readonly IServiceProvider _serviceProvider;

        public PFireDatabase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<User> InsertUser(string username, string password, string salt)
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
            return newUser;
        }

        public async Task InsertMutualFriend(User user1, User user2)
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

        public async Task InsertFriendRequest(User owner, User requestedUsername, string message)
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

        public async Task<User> QueryUser(int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>().FirstOrDefaultAsync(a => a.Id == userId);
        }

        public async Task<User> QueryUser(string username)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>().FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<List<User>> QueryUsers(string username)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>().Where(a => a.Username.Contains(username)).ToListAsync();
        }

        public async Task<List<User>> QueryFriends(User user)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>()
                               .Where(a => a.Id == user.Id)
                               .SelectMany(a => a.MyFriends)
                               .Where(x => x.Pending == false)
                               .Select(x => x.Them)
                               .ToListAsync();
        }

        public async Task<List<Friend>> QueryPendingFriendRequestsSelf(User user)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            return await reader.Query<User>().Where(a => a.Id == user.Id).SelectMany(x => x.MyFriends).Where(x => x.Pending).ToListAsync();
        }

        public async Task<List<Friend>> QueryPendingFriendRequests(User user)
        {
            using var scope = _serviceProvider.CreateScope();
            var reader = scope.ServiceProvider.GetRequiredService<IReader>();

            //TODO: use Model instead of Entity
            return await reader.Query<User>().Where(a => a.Id == user.Id).SelectMany(x => x.FriendsOf).Where(x => x.Pending).ToListAsync();
        }

        public async Task DeletePendingFriendRequest(params Friend[] pendingFriendRequestIds)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();
            var pendingFriendRequests = databaseContext.Set<Friend>();

            foreach (var pendingFriendRequest in pendingFriendRequestIds)
            {
                var pendingFriendRequest1 = await pendingFriendRequests.FindAsync(pendingFriendRequest.MeId, pendingFriendRequest.ThemId);
                var pendingFriendRequest2 = await pendingFriendRequests.FindAsync(pendingFriendRequest.ThemId, pendingFriendRequest.MeId);

                pendingFriendRequests.Remove(pendingFriendRequest1);
                pendingFriendRequests.Remove(pendingFriendRequest2);
            }

            await databaseContext.SaveChangesAsync();
        }

        public async Task UpdateNickname(User user, string nickname)
        {
            using var scope = _serviceProvider.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();

            var userToUpdate = await databaseContext.Set<User>().SingleAsync(x => x.Id == user.Id);
            userToUpdate.Nickname = nickname;

            await databaseContext.SaveChangesAsync();
        }
    }
}
