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
    }
}
