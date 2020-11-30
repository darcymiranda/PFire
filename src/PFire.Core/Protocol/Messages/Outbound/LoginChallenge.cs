using PFire.Core.Protocol.Messages;
using PFire.Session;

namespace PFire.Protocol.Messages.Outbound
{
    public sealed class LoginChallenge : XFireMessage
    {
        public LoginChallenge() : base(XFireMessageType.LoginChallenge) {  }

        [XMessageField("salt")]
        public string Salt { get; private set; }

      
        public override void Process(XFireClient context)
        {
            Salt = context.Salt;
        }
    }
}
