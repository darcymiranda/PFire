﻿using SQLite;
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
        public int FriendId { get; set; }
        public int UserId { get; set; }
        public int FriendUserId { get; set; }

        public static Friend New(int userId, int friendUserId)
        {
            var friend = new Friend {UserId = userId, FriendUserId = friendUserId};
            return friend;
        }
    }
}
