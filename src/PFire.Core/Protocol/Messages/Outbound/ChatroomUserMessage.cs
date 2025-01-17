using System.Collections.Generic;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomUserMessage : XFireMessage
    {
        public ChatroomUserMessage(byte[] chatId, int userid, string message) : base(XFireMessageType.ChatroomMessage) {
            ChatId = chatId;
            UserId = userid;
            Message = message;
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }

        [XMessageField(0x01)]
        public int UserId { get; set; }

        [XMessageField(0x2e)]
        public string Message { get; set; }

        public override Task Process(IXFireClient context)
        {
            return Task.CompletedTask;
        }
    }
}
