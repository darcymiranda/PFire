using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class FriendRemoval : XFireMessage
    {
        public FriendRemoval() : base(XFireMessageType.FriendRemoval) { }

        [XMessageField("userid")]
        public int UserId { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var friend = await context.Server.Database.QueryUser(UserId);
            await context.Server.Database.RemoveFriend(context.User, friend);
            await context.SendAndProcessMessage(new FriendRemoved(UserId));

            var friendSession = context.Server.GetSession(friend);
            if (friendSession != null)
            {
                await friendSession.SendAndProcessMessage(new FriendRemoved(context.User.Id));
            }
        }
    }
}
