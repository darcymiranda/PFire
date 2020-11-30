using PFire.Core.Protocol.Messages;

namespace PFire.Protocol.Messages.Inbound
{
    public sealed class Unknown10 : XFireMessage
    {
        public Unknown10() : base(XFireMessageType.Unknown10) { }
    }
}
