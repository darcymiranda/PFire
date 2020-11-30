using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
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
