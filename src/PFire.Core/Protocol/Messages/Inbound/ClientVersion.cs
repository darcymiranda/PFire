using PFire.Core.Protocol.Messages;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;

namespace PFire.Protocol.Messages.Inbound
{
    public sealed class ClientVersion : XFireMessage
    {
        public ClientVersion() : base(XFireMessageType.ClientVersion) { }

        [XMessageField("version")]
        public int Version { get; set; }

        [XMessageField("major_version")]
        public int MajorVersion { get; set; }

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
