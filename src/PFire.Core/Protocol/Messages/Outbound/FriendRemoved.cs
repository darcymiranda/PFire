using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendRemoved : XFireMessage
    {
        public FriendRemoved(uint userId) : base(XFireMessageType.FriendRemoved)
        {
            UserId = userId;
        }

        [XMessageField("userid")]
        public uint UserId { get; set; }
    }
}
