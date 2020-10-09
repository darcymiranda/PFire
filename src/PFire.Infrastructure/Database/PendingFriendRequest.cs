using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Database
{
    public class PendingFriendRequest
    {
        [PrimaryKey, AutoIncrement]
        public int PendingFriendRequestId { get; set; }
        public int UserId { get; set; }
        public int FriendUserId { get; set; }
        public string Message { get; set; }

        public static PendingFriendRequest New(int userId, int friendUserId, string message)
        {
            var friendRequest = new PendingFriendRequest
            {
                UserId = userId, FriendUserId = friendUserId, Message = message
            };
            return friendRequest;
        }
    }
}
