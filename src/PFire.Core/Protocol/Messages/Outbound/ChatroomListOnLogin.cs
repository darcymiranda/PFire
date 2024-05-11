using System.Collections.Generic;
using PFire.Core.Session;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomListOnLogin : XFireMessage
    {
        public ChatroomListOnLogin() : base(XFireMessageType.ChatRooms)
        {
            ChatIds = new List<byte[]>();
        }

        [XMessageField(0x04)]
        public List<byte[]> ChatIds { get; set; }

        public override async Task Process(IXFireClient context)
        {
            ChatIds = await context.Server.Database.GetChatrooms();
        }
    }
}
