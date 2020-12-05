using SQLite;

namespace PFire.Infrastructure.Database
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

        public User() { }

        public User(string username, string password, string salt)
        {
            Username = username;
            Nickname = username;
            Password = password;
            Salt = salt;
        }

        public override bool Equals(object obj)
        {
            if(obj.GetType() == typeof(User))
            {
                return Equals(obj as User);
            }

            return false;
        }

        public virtual bool Equals(User other)
        {
            return other != null && UserId == other.UserId;
        }

        public override int GetHashCode()
        {
            return UserId;
        }

        public static bool operator ==(User user1, User user2)
        {
            if (ReferenceEquals(user1, null))
            {
                return ReferenceEquals(user2, null);
            }

            return user1.Equals(user2);
        }

        public static bool operator !=(User user1, User user2)
        {
            return !(user1 == user2);
        }
    }
}
