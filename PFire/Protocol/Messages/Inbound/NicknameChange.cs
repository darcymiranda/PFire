using PFire.Protocol.Messages.Outbound;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Inbound
{
    public class NicknameChange : IMessage
    {
        [XFireAttributeDef("nick")]
        public string Nickname { get; private set; }

        private const int MAX_LENGTH = 35;

        public short MessageTypeId
        {
            get { return 14; }
        }

        public void Process(Context context)
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
                if(friendSession != null)
                {
                    friendSession.SendAndProcessMessage(updatedFriendsList);
                }
            });
        }
    }
}
