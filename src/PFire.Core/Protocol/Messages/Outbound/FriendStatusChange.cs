using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class FriendStatusChange : IMessage
    {
        [XFireAttributeDef("sid")]
        public List<Guid> SessionIds { get; private set; }

        [XFireAttributeDef("msg")]
        public List<string> Messages { get; private set; }

        public short MessageTypeId
        {
            get { return 154; }
        }

        public FriendStatusChange(Guid sessionId, string message)
        {
            SessionIds = new List<Guid>() { sessionId };
            Messages = new List<string>() { message };

        }

        public void Process(Context context)
        {

        }
    }
}
