using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class StatusChange : XFireMessage
    {
        public StatusChange() : base(XFireMessageType.StatusChange) {}

        [XMessageField(0x2e)]
        public string Message { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var statusChange = new FriendStatusChange(context.SessionId, Message);
            var friends = context.Server.Database.QueryFriends(context.User);
            foreach (var friend in friends)
            {
                var friendSession = context.Server.GetSession(friend);
                if (friendSession != null)
                {
                    await friendSession.SendAndProcessMessage(statusChange);
                }
            }
        }
    }
}
