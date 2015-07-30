using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class FriendsStatus : IMessage
    {
        [XFireAttributeDef(0x01)]
        public List<int> UserIds { get; private set; }

        [XFireAttributeDef(0x03)]
        public List<Guid> SessionIds { get; private set; }

        [XFireAttributeDef(0x0b)]
        public byte Unknown { get; private set; }

        public short MessageTypeId
        {
            get { return 132; }
        }

        private User owner;
        public FriendsStatus(User owner)
        {
            this.owner = owner;
            UserIds = new List<int>();
            SessionIds = new List<Guid>();
        }

        public void Process(Context context)
        {
            var friends = context.Server.Database.QueryFriends(context.User);
            friends.ForEach(a =>
            {
                UserIds.Add(a.UserId);

                var otherSession = context.Server.GetSession(a);
                SessionIds.Add(otherSession == null ? Guid.Empty : otherSession.SessionId);
            });
        }
    }
}
