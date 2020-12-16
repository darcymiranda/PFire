using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class FriendRequest : XFireMessage
    {
        public FriendRequest() : base(XFireMessageType.FriendRequest) {}

        [XMessageField("name")]
        public string Username { get; set; }

        [XMessageField("msg")]
        public string Message { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var recipient = await context.Server.Database.QueryUser(Username);
            var invite = new FriendInvite(context.User.Username, context.User.Nickname, Message);
            await invite.Process(context);

            await context.Server.Database.InsertFriendRequest(context.User, recipient, Message);

            var recipientSession = context.Server.GetSession(recipient);
            if (recipientSession != null)
            {
                await recipientSession.SendMessage(invite);
            }
        }
    }
}
