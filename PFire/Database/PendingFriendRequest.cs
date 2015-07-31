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
        public int SequenceId { get; private set; }
        public int UserId { get; private set; }
        public int FriendUserId { get; private set; }
        public string Message { get; private set; }

        public static PendingFriendRequest New(int userId, int friendUserId, string message)
        {
            PendingFriendRequest friendRequest = new PendingFriendRequest();
            friendRequest.UserId = userId;
            friendRequest.FriendUserId = friendUserId;
            friendRequest.Message = message;
            return friendRequest;
        }
    }
}
