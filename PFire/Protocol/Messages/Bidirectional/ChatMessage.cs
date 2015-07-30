using PFire.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Bidirectional
{
    public class ChatMessage : IMessage
    {
        [XFireAttributeDef("sid")]
        public Guid SessionId { get; private set; }
        
        [XFireAttributeDef("peermsg")]
        public Dictionary<string, dynamic> MessagePayload { get; private set; }

        public short MessageTypeId
        {
            get { return 133; }
        }

        // TODO: Create test for this message so we can refactor and build this message the same way as the others to avoid the switch statement
        // TODO: How to tell the client we didn't receive the ACK?
        // TODO: P2P stuff???
        public void Process(Context context)
        {
            var otherSession = context.Server.GetSession(SessionId);
            var messageType = (byte)MessagePayload["msgtype"];

            switch (messageType)
            {
                case 0:
                    var responseAck = BuildAckResponse(otherSession.SessionId);
                    context.SendMessage(responseAck);

                    var chatMsg = BuildChatMessageResponse(context.SessionId);
                    otherSession.SendMessage(chatMsg);
                    break;

                case 1:
                    Debug.WriteLine("NOT BUILT: Got ACK for session " + context.SessionId);
                    break;

                case 2:
                    Debug.WriteLine("NOT BUILT: Got P2P info for session " + context.SessionId);
                    break;

                case 3:
                    var typingMsg = BuildChatMessageResponse(context.SessionId);
                    otherSession.SendMessage(typingMsg);
                    break;
            }
        }

        private ChatMessage BuildChatMessageResponse(Guid sessionId)
        {
            var response = new ChatMessage();
            response.SessionId = sessionId;
            response.MessagePayload = new Dictionary<string, dynamic>(MessagePayload);
            return response;
        }

        private ChatMessage BuildAckResponse(Guid sessionId)
        {
            var ack = new ChatMessage();
            ack.SessionId = sessionId;
            ack.MessagePayload = new Dictionary<string, dynamic>();
            ack.MessagePayload.Add("msgtype", 1);
            ack.MessagePayload.Add("imindex", (int)MessagePayload["imindex"]);
            return ack;
        }
    }
}
