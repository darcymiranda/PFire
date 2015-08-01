using PFire.Database;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Database
{
    public class PFireDatabase : SQLiteConnection
    {
        public PFireDatabase() : base("pfiredb")
        {
            CreateTable<User>();
            CreateTable<Friend>();
            CreateTable<PendingFriendRequest>();
            System.IO.File.WriteAllText(@"C:\test.txt", System.IO.Directory.GetCurrentDirectory());
        }

        public bool QueryUsernameExists(string username)
        {
            return Table<User>().Any(a => a.Username == username);
        }

        public User InsertUser(string username, string password, string salt)
        {
            var id = Insert(User.New(username, password, salt));
            return QueryUser(id);
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
    }
}
