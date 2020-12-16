using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PFire.Core.Protocol.Messages.MessageEnums;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Bidirectional
{
    internal sealed class ChatMessage : XFireMessage
    {
        // the id of this message is the one that the original code base used
        // technically this is a server routed chat message
        public ChatMessage() : base(XFireMessageType.ServerChatMessage) {}

        [XMessageField("sid")]
        public Guid SessionId { get; set; }

        [XMessageField("peermsg")]
        public Dictionary<string, dynamic> MessagePayload { get; set; }

        // TODO: Create test for this message so we can refactor and build this message the same way as the others to avoid the switch statement
        // TODO: How to tell the client we didn't receive the ACK?
        // TODO: P2P stuff???
        public override async Task Process(IXFireClient context)
        {
            var otherSession = context.Server.GetSession(SessionId);
            if (otherSession == null)
            {
                return;
            }

            var messageType = (ChatMessageType)(byte)MessagePayload["msgtype"];

            switch (messageType)
            {
                case ChatMessageType.Content:
                    var responseAck = BuildAckResponse(otherSession.SessionId);
                    await context.SendMessage(responseAck);

                    var chatMsg = BuildChatMessageResponse(context.SessionId);
                    await otherSession.SendMessage(chatMsg);
                    break;

                case ChatMessageType.TypingNotification:
                    var typingMsg = BuildChatMessageResponse(context.SessionId);
                    await otherSession.SendMessage(typingMsg);
                    break;

                default:
                    context.Logger.LogDebug($"NOT BUILT: Got {messageType} for session: {context.SessionId}");
                    break;
            }
        }

        private ChatMessage BuildChatMessageResponse(Guid sessionId)
        {
            return new ChatMessage
            {
                SessionId = sessionId,
                MessagePayload = new Dictionary<string, dynamic>(MessagePayload)
            };
        }

        private ChatMessage BuildAckResponse(Guid sessionId)
        {
            var ack = new ChatMessage
            {
                SessionId = sessionId,
                MessagePayload = new Dictionary<string, dynamic>()
            };

            ack.MessagePayload.Add("imindex", (int)MessagePayload["imindex"]);
            return ack;
        }
    }
}
