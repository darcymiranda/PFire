using PFire.Core.Protocol.Messages;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;

namespace PFire.Protocol.Messages.Inbound
{
    public sealed class ClientConfiguration : XFireMessage
    {
        public ClientConfiguration() : base(XFireMessageType.ClientConfiguration) { }

        [XMessageField("lang")]
        public string Language { get; set; }

        [XMessageField("skin")]
        public string Skin { get; set; }

        [XMessageField("theme")]
        public string Theme { get; set; }

        [XMessageField("partner")]
        public string Partner { get; set; }

        public override void Process(XFireClient context)
        {
            context.SendAndProcessMessage(new Did());
        }
    }
}
