using PFire.Protocol.Messages.Outbound;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Inbound
{
    public class StatusChange : IMessage
    {
        [XFireAttributeDef(0x2e)]
        public string Message { get; private set; }

        public short MessageTypeId => 32;

        public void Process(Context context)
        {
            var statusChange = new FriendStatusChange(context.SessionId, Message);
            var friends = context.Server.Database.QueryFriends(context.User);
            friends.ForEach(friend =>
            {
                var friendSession = context.Server.GetSession(friend);
                friendSession?.SendAndProcessMessage(statusChange);
            });
        }
    }
}
