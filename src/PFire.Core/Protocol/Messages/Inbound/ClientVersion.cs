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

        public override void Process(XFireClient context)
        {
            var loginChallenge = new LoginChallenge();
            loginChallenge.Process(context);
            context.SendMessage(loginChallenge);
        }

        public override string ToString()
        {
            return $"[ClientVersion] v: {Version} mv: {MajorVersion}";
        }
    }
}
