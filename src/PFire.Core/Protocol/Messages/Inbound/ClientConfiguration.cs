using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class ClientConfiguration : XFireMessage
    {
        public ClientConfiguration() : base(XFireMessageType.ClientConfiguration) {}

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
