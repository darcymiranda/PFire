using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;

namespace PFire.Infrastructure.Database
{
    public class PFireDatabase : SQLiteConnection
    {
        private const string DatabaseName = "pfiredb.sqlite";
        public PFireDatabase(string baseDirectory) : base(Path.Combine(baseDirectory, DatabaseName))
        {
            CreateTable<User>();
            CreateTable<Friend>();
            CreateTable<PendingFriendRequest>();
        }

        public User InsertUser(string username, string password, string salt)
        {
            var newUser = User.New(username, password, salt);
            Insert(newUser);
            return QueryUser(newUser.UserId);
        }

        public void InsertMutualFriend(User user1, User user2)
        {
            Insert(Friend.New(user1.UserId, user2.UserId));
            Insert(Friend.New(user2.UserId, user1.UserId));
        }

        public void InsertFriendRequest(User owner, string requestedUsername, string message)
        {
            Insert(PendingFriendRequest.New(owner.UserId, QueryUser(requestedUsername).UserId, message));
        }

        public User QueryUser(int userId)
        {
            return Table<User>().FirstOrDefault(a => a.UserId == userId);
        }

        public User QueryUser(string username)
        {
            return Table<User>().FirstOrDefault(a => a.Username == username);
        }

        public List<User> QueryUsers(string username)
        {
            return Table<User>().Where(a => a.Username.Contains(username)).ToList();
        }

        public List<User> QueryFriends(User user)
        {
            var friendMatches = Table<Friend>().Where(a => a.UserId == user.UserId).ToList();
            var friends = friendMatches.Select(a => a.FriendUserId).ToList();
            return Table<User>().Where(a => friends.Contains(a.UserId)).ToList();
        }

        public List<PendingFriendRequest> QueryPendingFriendRequestsSelf(User user)
        {
            return Table<PendingFriendRequest>().Where(a => a.UserId == user.UserId).ToList();
        }

        public List<PendingFriendRequest> QueryPendingFriendRequests(User otherUser)
        {
            return Table<PendingFriendRequest>().Where(a => a.FriendUserId == otherUser.UserId).ToList();
        }

        public void DeletePendingFriendRequest(params int[] sequenceIds)
        {
            sequenceIds.ToList().ForEach(a => Delete<PendingFriendRequest>(a));
        }

        public void UpdateNickname(User user, string nickname)
        {
            Execute("UPDATE User SET Nickname = ? WHERE UserId = ?", nickname, user.UserId);
        }
    }
}
