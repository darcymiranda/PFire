using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PFire.Common.Models;
using SQLite;

namespace PFire.Infrastructure.Database
{
    public interface IPFireDatabase
    {
        Task<User> InsertUser(string username, string password, string salt);
        Task InsertMutualFriend(User user1, User user2);
        Task InsertFriendRequest(User owner, string requestedUsername, string message);
        Task<User> QueryUser(int userId);
        Task<User> QueryUser(string username);
        Task<List<User>> QueryUsers(string username);
        Task<List<User>> QueryFriends(User user);
        Task<List<PendingFriendRequest>> QueryPendingFriendRequestsSelf(User user);
        Task<List<PendingFriendRequest>> QueryPendingFriendRequests(User otherUser);
        Task DeletePendingFriendRequest(params int[] sequenceIds);
        Task UpdateNickname(User user, string nickname);
    }

    internal class PFireDatabase : SQLiteConnection, IPFireDatabase
    {
        public PFireDatabase(IOptions<DatabaseSettings> databaseSettings, IHostEnvironment hostEnvironment) : base(Path.Combine(hostEnvironment.ContentRootPath,
            databaseSettings.Value.Name))
        {
            CreateTable<User>();
            CreateTable<Friend>();
            CreateTable<PendingFriendRequest>();
        }

        public async Task<User> InsertUser(string username, string password, string salt)
        {
            await Task.Yield();

            var newUser = new User(username, password, salt);
            Insert(newUser);
            return await QueryUser(newUser.UserId);
        }

        public async Task InsertMutualFriend(User user1, User user2)
        {
            await Task.Yield();
            Insert(new Friend(user1.UserId, user2.UserId));
            Insert(new Friend(user2.UserId, user1.UserId));
        }

        public async Task InsertFriendRequest(User owner, string requestedUsername, string message)
        {
            await Task.Yield();

            var queryUser = await QueryUser(requestedUsername);
            Insert(new PendingFriendRequest(owner.UserId, queryUser.UserId, message));
        }

        public async Task<User> QueryUser(int userId)
        {
            await Task.Yield();

            return Table<User>().FirstOrDefault(a => a.UserId == userId);
        }

        public async Task<User> QueryUser(string username)
        {
            await Task.Yield();

            return Table<User>().FirstOrDefault(a => a.Username == username);
        }

        public async Task<List<User>> QueryUsers(string username)
        {
            await Task.Yield();

            return Table<User>().Where(a => a.Username.Contains(username)).ToList();
        }

        public async Task<List<User>> QueryFriends(User user)
        {
            await Task.Yield();

            var friendMatches = Table<Friend>().Where(a => a.UserId == user.UserId).ToList();
            var friends = friendMatches.Select(a => a.FriendUserId).ToList();
            return Table<User>().Where(a => friends.Contains(a.UserId)).ToList();
        }

        public async Task<List<PendingFriendRequest>> QueryPendingFriendRequestsSelf(User user)
        {
            await Task.Yield();

            return Table<PendingFriendRequest>().Where(a => a.UserId == user.UserId).ToList();
        }

        public async Task<List<PendingFriendRequest>> QueryPendingFriendRequests(User otherUser)
        {
            await Task.Yield();

            return Table<PendingFriendRequest>().Where(a => a.FriendUserId == otherUser.UserId).ToList();
        }

        public async Task DeletePendingFriendRequest(params int[] sequenceIds)
        {
            await Task.Yield();

            sequenceIds.ToList().ForEach(a => Delete<PendingFriendRequest>(a));
        }

        public async Task UpdateNickname(User user, string nickname)
        {
            await Task.Yield();

            Execute("UPDATE User SET Nickname = ? WHERE UserId = ?", nickname, user.UserId);
        }
    }
}
