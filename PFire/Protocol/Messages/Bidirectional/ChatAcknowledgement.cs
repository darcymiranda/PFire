using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Bidirectional
{
    public class ChatAcknowledgement : IMessage
    {
        public short MessageTypeId
        {
            get { return 1; }
        }

        public void Process(Context context)
        {
            throw new NotImplementedException();
        }
    }
}
