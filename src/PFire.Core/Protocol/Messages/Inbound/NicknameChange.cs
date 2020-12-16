using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class NicknameChange : XFireMessage
    {
        private const int MAX_LENGTH = 35;

        public NicknameChange() : base(XFireMessageType.NicknameChange) {}

        [XMessageField("nick")]
        public string Nickname { get; set; }

        public override async Task Process(IXFireClient context)
        {
            if (Nickname.Length > MAX_LENGTH)
            {
                Nickname = Nickname.Substring(0, MAX_LENGTH);
            }

            await context.Server.Database.UpdateNickname(context.User, Nickname);

            var updatedFriendsList = new FriendsList(context.User);
            var queryFriends = await context.Server.Database.QueryFriends(context.User);
            foreach (var friend in queryFriends)
            {
                var friendSession = context.Server.GetSession(friend);
                if (friendSession != null)
                {
                    await friendSession.SendAndProcessMessage(updatedFriendsList);
                }
            }
        }
    }
}
