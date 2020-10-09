using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;

namespace PFire.Protocol.Messages.Inbound
{
    public class ClientVersion : IMessage
    {
        [XFireAttributeDef("version")]
        public int Version { get; private set;}

        [XFireAttributeDef("major_version")]
        public int MajorVersion { get; private set; }

        public short MessageTypeId => 3;

        public void Process(Context context)
        {
            var loginChallenge = new LoginChallenge();
            loginChallenge.Process(context);
            context.SendMessage(loginChallenge);
        }

        public override string ToString()
        {
            return new StringBuilder().Append("[").Append(GetType().Name).Append("] ")
                .Append("version: ").Append(Convert.ToString(Version)).Append(", ")
                .Append("major_version: ").Append(Convert.ToString(MajorVersion)).ToString();
        }
    }
}
