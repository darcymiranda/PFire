using PFire.Session;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire
{
    public class PFireDatabase : SQLiteConnection
    {
        public PFireDatabase() : base("pfiredb")
        {
            CreateTable<User>();
            CreateTable<Friend>();
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
    }
}
