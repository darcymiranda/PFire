using PFire.Protocol.Messages.Outbound;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Inbound
{
    public class FriendRequest : IMessage
    {

        [XFireAttributeDef("name")]
        public string Username { get; private set;}

        [XFireAttributeDef("msg")]
        public string Message { get; private set; }

        public short MessageTypeId
        {
            get { return 6; }
        }

        public void Process(Context context)
        {
            var recipient = context.Server.Database.QueryUser(Username);
            var invite = new FriendInvite(context.User.Username, context.User.Nickname, Message);
            invite.Process(context);

            context.Server.Database.InsertFriendRequest(context.User, Username, Message);

            var recipientSession = context.Server.GetSession(recipient);
            if (recipientSession != null)
            {
                recipientSession.SendMessage(invite);
            }
        }
    }
}
