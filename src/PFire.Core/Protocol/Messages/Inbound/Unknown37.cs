using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Inbound
{
    public class Unknown37 : IMessage
    {
        public short MessageTypeId => 37;

        public void Process(Context context)
        {
            // Nothing
        }
    }
}
