using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class StatusChange : XFireMessage
    {
        public StatusChange() : base(XFireMessageType.StatusChange) {}

        [XMessageField(0x2e)]
        public string Message { get; private set; }

        public override void Process(XFireClient context)
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
