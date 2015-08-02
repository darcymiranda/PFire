using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Inbound
{
    public class Unknown10 : IMessage
    {
        public short MessageTypeId
        {
            get { return 10; }
        }

        public void Process(Context context)
        {
        }
    }
}
