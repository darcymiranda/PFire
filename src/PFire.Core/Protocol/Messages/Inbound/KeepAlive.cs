using PFire.Core.Protocol.Messages;

namespace PFire.Protocol.Messages.Inbound
{
    public sealed class KeepAlive : XFireMessage
    {
        public KeepAlive() : base(XFireMessageType.KeepAlive) { } 
    }
}
