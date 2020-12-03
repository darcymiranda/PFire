namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class KeepAlive : XFireMessage
    {
        public KeepAlive() : base(XFireMessageType.KeepAlive) {}
    }
}
