using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;
using PFire.Infrastructure.Entities;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendsOfFriendsInfo : XFireMessage
    {
        private readonly List<UserModel> SidList = new List<UserModel>();

        public FriendsOfFriendsInfo(List<UserModel> users) : base(XFireMessageType.FriendsOfFriendsInfo)
        {
            SidList = users;
        }

        [XMessageField("fnsid")]
        public List<Guid> Fnsid { get; } = new List<Guid>();
        [XMessageField("userid")]
        public List<int> Userid { get; } = new List<int>();
        [XMessageField("name")]
        public List<string> Name { get; } = new List<string>();
        [XMessageField("nick")]
        public List<string> Nick { get; } = new List<string>();
        [XMessageField("friends")]
        public List<List<int>> Friends { get; } = new List<List<int>>();

        public override async Task Process(IXFireClient client)
        {
            var sessions = SidList
            .Select(sid => client.Server.GetSession(sid))
            .Where(session => session != null)
            .ToList();

            if (sessions.Count != 0)
            {
                foreach (var session in sessions)
                {
                    Fnsid.Add(session.SessionId);
                    Userid.Add(session.User.Id);
                    Name.Add(session.User.Username);
                    Nick.Add(session.User.Nickname);

                    var friendIds = (await client.Server.Database.QueryFriends(session.User))
                        .Select(friend => friend.Id)
                        .ToList();

                    Friends.Add(friendIds);
                }
            }
        }
    }
}
