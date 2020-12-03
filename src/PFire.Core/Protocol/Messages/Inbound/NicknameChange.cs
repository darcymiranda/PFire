using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class NicknameChange : XFireMessage
    {
        private const int MAX_LENGTH = 35;

        public NicknameChange() : base(XFireMessageType.NicknameChange) {}

        [XMessageField("nick")]
        public string Nickname { get; private set; }

        public override void Process(IXFireClient context)
        {
            if (Nickname.Length > MAX_LENGTH)
            {
                Nickname = Nickname.Substring(0, MAX_LENGTH);
            }

            context.Server.Database.UpdateNickname(context.User, Nickname);

            var updatedFriendsList = new FriendsList(context.User);
            context.Server.Database.QueryFriends(context.User)
                   .ForEach(friend =>
                   {
                       var friendSession = context.Server.GetSession(friend);
                       friendSession?.SendAndProcessMessage(updatedFriendsList);
                   });
        }
    }
}
