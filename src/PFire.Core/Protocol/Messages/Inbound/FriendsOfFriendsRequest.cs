using Microsoft.VisualBasic;
using PFire.Core.Models;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class FriendsOfFriendsRequest : XFireMessage
    {
        public FriendsOfFriendsRequest() : base(XFireMessageType.FriendsOfFriendsRequest) { }

        [XMessageField("sid")]
        public List<object> Sids { get; set; }

        public override Task Process(IXFireClient client)
        {
            var friendsOfFriends = Sids.Cast<object>()
                                       .Where(sid => sid != null)
                                       .Select(sid => new Guid(sid.ToString()))
                                       .Select(sessionId => client.Server.GetSession(sessionId))
                                       .Where(session => session != null)
                                       .Select(session => session.User)
                                       .ToList();

            if (friendsOfFriends.Count != 0)
            {
                client.SendAndProcessMessage(new FriendsOfFriendsInfo(friendsOfFriends));
            }
            return Task.CompletedTask;
        }
    }
}
