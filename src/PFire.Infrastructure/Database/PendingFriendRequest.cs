using SQLite;

namespace PFire.Infrastructure.Database
{
    public class PendingFriendRequest
    {
        [PrimaryKey, AutoIncrement]
        public int PendingFriendRequestId { get; set; }
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
        public string Message { get; set; }

        public PendingFriendRequest() { }
        public PendingFriendRequest(int userId, int friendUserId, string message)
        {
            UserId = userId;
            FriendUserId = friendUserId;
            Message = message;
        }
    }
}
