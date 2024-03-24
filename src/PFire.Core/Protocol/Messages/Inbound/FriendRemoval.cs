using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class FriendRemoval : XFireMessage
    {
        public FriendRemoval() : base(XFireMessageType.FriendRemoval) { }

        [XMessageField("userid")]
        public int userid { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var friend = context.Server.Database.QueryUser(userid);
            await context.Server.Database.RemoveFriend(context.User, friend.Result);
            await context.SendAndProcessMessage(new FriendRemoved(userid));

            var friendSession = context.Server.GetSession(friend.Result);
            if (friendSession != null)
            {
                await friendSession.SendAndProcessMessage(new FriendRemoved(context.User.Id));
            }
        }
    }
}
