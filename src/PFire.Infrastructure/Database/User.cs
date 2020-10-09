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
        public int UserId { get; set; }

        [Unique]
        public string Username { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public string Nickname { get; set; }

        public static User New(string username, string password, string salt)
        {
            var user = new User {Username = username, Nickname = username, Password = password, Salt = salt};
            return user;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public virtual bool Equals(User other)
        {
            if (other == null) return false;
            return this.UserId == other.UserId;
        }

        public override int GetHashCode()
        {
            return UserId;
        }

        public static bool operator ==(User user1, User user2)
        {
            if (object.ReferenceEquals(user1, null))
            {
                return object.ReferenceEquals(user2, null);
            }
            return user1.Equals(user2);
        }

        public static bool operator !=(User user1, User user2)
        {
            return !(user1 == user2);
        }
    }
}
