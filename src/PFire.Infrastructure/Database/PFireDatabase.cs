using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace PFire.Infrastructure.Database
{
    public interface IPFireDatabase
    {
        User InsertUser(string username, string password, string salt);
        void InsertMutualFriend(User user1, User user2);
        void InsertFriendRequest(User owner, string requestedUsername, string message);
        User QueryUser(int userId);
        User QueryUser(string username);
        List<User> QueryUsers(string username);
        List<User> QueryFriends(User user);
        List<PendingFriendRequest> QueryPendingFriendRequestsSelf(User user);
        List<PendingFriendRequest> QueryPendingFriendRequests(User otherUser);
        void DeletePendingFriendRequest(int friendRequestId);
        void DeletePendingFriendRequests(IEnumerable<int> friendRequestIds);
        void UpdateNickname(User user, string nickname);
    }

    internal class PFireDatabase : SQLiteConnection, IPFireDatabase
    {
        public PFireDatabase(string databasePath) : base(databasePath)
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

        public void DeletePendingFriendRequest(int friendRequestId)
        {
            Delete<PendingFriendRequest>(friendRequestId);
        }

        public void DeletePendingFriendRequests(IEnumerable<int> friendRequestIds)
        {
            foreach (var friendRequestId in friendRequestIds)
            {
                DeletePendingFriendRequest(friendRequestId);
            }
        }

        public void UpdateNickname(User user, string nickname)
        {
            Execute("UPDATE User SET Nickname = ? WHERE UserId = ?", nickname, user.UserId);
        }
    }
}
