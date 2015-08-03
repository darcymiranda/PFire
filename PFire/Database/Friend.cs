using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Database
{
    public class Friend
    {
        [PrimaryKey, AutoIncrement]
        public int FriendId { get; private set; }
        public int UserId { get; private set; }
        public int FriendUserId { get; private set; }

        public static Friend New(int userId, int friendUserId)
        {
            Friend friend = new Friend();
            friend.UserId = userId;
            friend.FriendUserId = friendUserId;
            return friend;
        }
    }
}
