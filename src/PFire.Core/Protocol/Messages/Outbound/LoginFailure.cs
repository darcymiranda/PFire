namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class LoginFailure : XFireMessage
    {
        public LoginFailure() : base(XFireMessageType.LoginFailure) {}

        [XMessageField("reason")]
        public int Reason { get; set; }
    }
}
