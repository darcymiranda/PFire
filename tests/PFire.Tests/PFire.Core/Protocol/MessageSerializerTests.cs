using PFire.Core.Protocol;
using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.Messages.Inbound;
using Xunit;

namespace PFire.Tests.PFire.Core.Protocol
{
    public class MessageSerializerTests
    {
        [Theory]
        [InlineData(new byte[] { 0x0D, 0x00, 0x02, 0x05, 0x76, 0x61, 0x6C, 0x75, 0x65, 0x02, 0x00, 0x00, 0x00, 0x00, 0x05, 0x73, 0x74, 0x61, 0x74, 0x73, 0x04, 0x02, 0x00, 0x00 })]
        public void Deserialize_KeepAlive_ReturnsMessage(byte[] bytes)
        {
            var message = MessageSerializer.Deserialize(bytes);

            Assert.True(message.MessageTypeId == XFireMessageType.KeepAlive);
            Assert.True(typeof(KeepAlive) == message.GetType());

            var keepAlive = message as KeepAlive;
            Assert.True(keepAlive.Value == 0);
            Assert.True(keepAlive.Stats.Count == 0);
        }
    }
}
