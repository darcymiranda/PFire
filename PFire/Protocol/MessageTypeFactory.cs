using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.Messages;
using PFire.Protocol.Messages.Inbound;
using PFire.Protocol.Messages.Outbound;
using PFire.Protocol.Messages.Bidirectional;

namespace PFire.Protocol
{
    public class MessageTypeFactory
    {
        private static MessageTypeFactory instance;

        private Dictionary<short, IMessage> messages = new Dictionary<short, IMessage>();

        private MessageTypeFactory() {
            Add(new ClientVersion());
            Add(new LoginRequest());
            Add(new LoginFailure());
            Add(new LoginSuccess());
            Add(new ClientConfiguration());
            Add(new ClientPreferences());
            Add(new ConnectionInformation());
            Add(new Groups());
            Add(new GroupsFriends());
            Add(new ServerList());
            Add(new FriendsList());
            Add(new FriendsStatus());
            Add(new ChatRooms());
            Add(new GameInformation());
            Add(new KeepAlive());
            Add(new Did());
            Add(new ChatMessage());
            Add(new Unknown37());
            Add(new UserLookup());
        }

        private void Add(IMessage message)
        {
            messages.Add(message.MessageTypeId, message);
        }

        public Type GetMessageType(short type)
        {
            // Hack: Client sends message type of 2 for chat messages but expects message type of 133 on receive...
            if (type == 2)
            {
                return messages[133].GetType();
            }

            if (!messages.ContainsKey(type))
            {
                throw new UnknownMessageTypeException(type);
            }
            return messages[type].GetType();
        }

        public static MessageTypeFactory Instance
        {
            get
            {
                return instance ?? new MessageTypeFactory();
            }
        }
    }
}
