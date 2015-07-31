using PFire.Protocol.Messages.Outbound;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Inbound
{
    public class UserLookup : IMessage
    {
        [XFireAttributeDef("name")]
        public string Username { get; private set; }

        [XFireAttributeDef("fname")]
        public string FirstName { get; private set; }

        [XFireAttributeDef("lname")]
        public string LastName { get; private set; }

        [XFireAttributeDef("email")]
        public string Email { get; private set; }

        public short MessageTypeId
        {
            get { return 12; }
        }

        public void Process(Context context)
        {
            var result = new UserLookupResult(Username);
            context.SendAndProcessMessage(result);
        }
    }
}
