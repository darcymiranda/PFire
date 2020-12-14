﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendsSessionAssign : XFireMessage
    {
        private static readonly Guid FriendIsOffLineSessionId = Guid.Empty;
        private readonly UserModel _ownerUser;

        public FriendsSessionAssign(UserModel owner) : base(XFireMessageType.FriendsSessionAssign)
        {
            _ownerUser = owner;
            UserIds = new List<int>();
            SessionIds = new List<Guid>();
        }

        [XMessageField("userid")]
        public List<int> UserIds { get; }

        [XMessageField("sid")]
        public List<Guid> SessionIds { get; }

        [XMessageField(0x0b)]
        public byte Unknown { get; set; }

        public override async Task Process(IXFireClient client)
        {
            var friends = await client.Server.Database.QueryFriends(_ownerUser);
            foreach (var friend in friends)
            {
                var friendSession = client.Server.GetSession(friend);

                UserIds.Add(friend.Id);
                SessionIds.Add(friendSession?.SessionId ?? FriendIsOffLineSessionId);
            }
        }
    }
}
