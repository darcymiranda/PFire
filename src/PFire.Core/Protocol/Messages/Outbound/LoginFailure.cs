namespace PFire.Core.Protocol.Messages.Outbound
{
    public sealed class LoginFailure : XFireMessage
    {
        public LoginFailure() : base(XFireMessageType.LoginFailure) {  }

        [XMessageField("reason")]
        public int Reason { get; private set; }
    }
}
