﻿using System.Collections.Generic;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendsList : XFireMessage
    {
        private readonly UserModel _ownerUser;

        public FriendsList(UserModel owner) : base(XFireMessageType.FriendsList)
        {
            _ownerUser = owner;

            UserIds = new List<int>();
            Usernames = new List<string>();
            Nicks = new List<string>();
        }

        [XMessageField("userid")]
        public List<int> UserIds { get; }

        [XMessageField("friends")]
        public List<string> Usernames { get; }

        [XMessageField("nick")]
        public List<string> Nicks { get; }

        public override async Task Process(IXFireClient context)
        {
            var friends = await context.Server.Database.QueryFriends(_ownerUser);
            friends.ForEach(f =>
            {
                UserIds.Add(f.Id);
                Usernames.Add(f.Username);
                Nicks.Add(f.Nickname);
            });
        }
    }
}
