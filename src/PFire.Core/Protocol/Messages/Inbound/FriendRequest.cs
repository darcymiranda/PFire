using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class FriendRequest : XFireMessage
    {
        public FriendRequest() : base(XFireMessageType.FriendRequest) {}

        [XMessageField("name")]
        public string Username { get; private set; }

        [XMessageField("msg")]
        public string Message { get; private set; }

        public override void Process(XFireClient context)
        {
            var recipient = context.Server.Database.QueryUser(Username);
            var invite = new FriendInvite(context.User.Username, context.User.Nickname, Message);
            invite.Process(context);

            context.Server.Database.InsertFriendRequest(context.User, Username, Message);

            var recipientSession = context.Server.GetSession(recipient);
            recipientSession?.SendMessage(invite);
        }
    }
}
