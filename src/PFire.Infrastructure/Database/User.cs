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

        public static User New(string username, string password, string salt)
        {
            return new User {Username = username, Nickname = username, Password = password, Salt = salt};
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
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
