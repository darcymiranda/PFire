using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class LoginFailure : IMessage
    {
        [XFireAttributeDef("reason")]
        public int Reason { get; private set; }

        public short MessageTypeId
        {
            get { return 129; }
        }

        public void Process(Context context)
        {
        }
    }
}
