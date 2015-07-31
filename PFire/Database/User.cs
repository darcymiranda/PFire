using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Database
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; private set; }

        [Unique]
        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Salt { get; private set; }

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

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public virtual bool Equals(User other)
        {
            return this.UserId == other.UserId;
        }

        public override int GetHashCode()
        {
            return UserId;
        }

        public static bool operator == (User user1, User user2)
        {
            if (object.ReferenceEquals(user1, null))
            {
                if (object.ReferenceEquals(user2, null))
                {
                    return true;
                }
                return false;
            }
            return user1.Equals(user2);
        }

        public static bool operator != (User user1, User user2)
        {
            return !(user1 == user2);
        }
    }
}
