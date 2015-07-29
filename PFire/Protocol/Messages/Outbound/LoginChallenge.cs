using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.XFireAttributes;
using PFire.Session;

namespace PFire.Protocol.Messages.Outbound
{
    public class LoginChallenge : IMessage
    {
        [XFireAttributeDef("salt")]
        public string Salt { get; private set; }

        public short MessageTypeId
        {
            get { return 128; }
        }

        public void Process(Context context)
        {
            Salt = context.Salt;
        }
    }
}
