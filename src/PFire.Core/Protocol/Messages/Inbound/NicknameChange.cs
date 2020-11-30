using PFire.Core.Protocol.Messages;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;

namespace PFire.Protocol.Messages.Inbound
{
    public sealed class NicknameChange : XFireMessage
    {
        public NicknameChange() : base(XFireMessageType.NicknameChange) { } 

        [XMessageField("nick")]
        public string Nickname { get; private set; }

        private const int MAX_LENGTH = 35;

        public override void Process(XFireClient context)
        {
            if (Nickname.Length > MAX_LENGTH)
            {
                Nickname = Nickname.Substring(0, MAX_LENGTH);
            }

            context.Server.Database.UpdateNickname(context.User, Nickname);

            var updatedFriendsList = new FriendsList(context.User);
            context.Server.Database.QueryFriends(context.User).ForEach(friend =>
            {
                var friendSession = context.Server.GetSession(friend);
                friendSession?.SendAndProcessMessage(updatedFriendsList);
            });
        }
    }
}
