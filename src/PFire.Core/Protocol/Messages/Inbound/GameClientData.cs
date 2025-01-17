using System;
using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

/*
 * 
 *  Packet 35 - Game Client Data
 *  This is extra client info that is sent by a game that uses XFire SDK.
 *  That gives details in a very similar way to the modern Discord Rich Presence.
 * 
 */

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class GameClientData : XFireMessage
    {
        public GameClientData() : base(XFireMessageType.GameClientData) { }

        [XMessageField(0x5b)]
        public string ClientData { get; set; }

        public override async Task Process(IXFireClient context)
        {
            //TODO: Make this reachable via database and send it off when a friend logs in.
            // For now, send the data as we get it. Users who log in after the packet sends won't see it.
            Guid sessionId = context.SessionId;
            var friends = await context.Server.Database.QueryFriends(context.User);
            foreach (var friend in friends)
            {
                var otherSession = context.Server.GetSession(friend);
                if (otherSession != null)
                {
                    await otherSession.SendAndProcessMessage(new FriendGameClientData([sessionId], [ClientData]));
                }
            }
        }
    }
}
