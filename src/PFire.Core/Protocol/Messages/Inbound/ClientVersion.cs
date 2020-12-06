using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class ClientVersion : XFireMessage
    {
        public ClientVersion() : base(XFireMessageType.ClientVersion) {}

        [XMessageField("version")]
        public int Version { get; private set; }

        [XMessageField("major_version")]
        public int MajorVersion { get; private set; }

        public override Task Process(IXFireClient context)
        {
            var loginChallenge = new LoginChallenge();
            loginChallenge.Process(context);
            context.SendMessage(loginChallenge);

            return Task.CompletedTask;
        }

        public override string ToString()
        {
            return $"[ClientVersion] v: {Version} mv: {MajorVersion}";
        }
    }
}
