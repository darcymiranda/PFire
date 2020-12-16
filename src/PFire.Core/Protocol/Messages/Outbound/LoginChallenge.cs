using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class LoginChallenge : XFireMessage
    {
        public LoginChallenge() : base(XFireMessageType.LoginChallenge) {}

        [XMessageField("salt")]
        public string Salt { get; set; }

        public override Task Process(IXFireClient context)
        {
            Salt = context.Salt;

            return Task.CompletedTask;
        }
    }
}
