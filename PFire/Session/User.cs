using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Session
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; private set; }

        [Unique]
        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Salt { get; private set; }

        [Ignore]
        public string Nickname { get; private set; }

        public static User New(string username, string password, string salt)
        {
            User user = new User();
            user.Username = username;
            user.Nickname = username;
            user.Password = password;
            user.Salt = salt;
            return user;
        }
    }
}
