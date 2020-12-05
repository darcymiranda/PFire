namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class Did : XFireMessage
    {
        public Did() : base(XFireMessageType.Did)
        {
            // this is supposed to be read from the server
            Unknown = new byte[]
            {
                0xd1,
                0xc2,
                0x95,
                0x33,
                0x84,
                0xc4,
                0xcc,
                0xb2,
                0x31,
                0x50,
                0x1f,
                0x0e,
                0x43,
                0xc5,
                0x89,
                0x30,
                0xb2,
                0xa7,
                0x0e,
                0x4a,
                0xb6
            };
        }

        [XMessageField("did")]
        public byte[] Unknown { get; }
    }
}
