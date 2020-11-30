namespace PFire.Core.Protocol.Messages.Inbound
{
    public sealed class KeepAlive : XFireMessage
    {
        public KeepAlive() : base(XFireMessageType.KeepAlive) { } 
    }
}
