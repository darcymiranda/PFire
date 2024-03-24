using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendRemoved : XFireMessage
    {
        public FriendRemoved(int userId) : base(XFireMessageType.FriendRemoved)
        {
            userid = userId;
        }

        [XMessageField("userid")]
        public int userid { get; set; }
    }
}
