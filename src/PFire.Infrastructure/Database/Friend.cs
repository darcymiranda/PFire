using SQLite;

namespace PFire.Infrastructure.Database
{
    public class Friend
    {
        [PrimaryKey, AutoIncrement]
        public int FriendId { get; set; }
        public int UserId { get; set; }
        public int FriendUserId { get; set; }

        public Friend() { }

        public Friend(int userId, int friendUserId)
        {
            UserId = userId;
            FriendUserId = friendUserId;
        }
    }
}
