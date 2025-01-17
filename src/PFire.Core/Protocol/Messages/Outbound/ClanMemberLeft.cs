/* 
 * Packet 169 - Clan Member Left
 * Clan ID and Member User Id of who left. Pretty self-explainatory.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanMemberLeft : XFireMessage
    {
        public ClanMemberLeft(int clanId, int userId) : base(XFireMessageType.ClanMemberLeft)
        {
            ClanId = clanId;
            UserId = userId;
        }

        [XMessageField(0x6c)]
        public int ClanId { get; set; }
        [XMessageField(0x01)]
        public int UserId { get; set; }
    }
}
