/* 
 * Packet 162 - Clan Member Name Change
 * Changes the clan name based on user id and clan id.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanMemberNameChange : XFireMessage
    {
        public ClanMemberNameChange(int clanId, int userId, string newName) : base(XFireMessageType.ClanMemberNameChange)
        {
            ClanId = clanId;
            UserId = userId;
            NewNameWithinClan = newName;
        }

        [XMessageField(0x6c)]
        public int ClanId { get; set; }
        [XMessageField(0x01)]
        public int UserId { get; set; }
        [XMessageField(0x0d)]
        public string NewNameWithinClan { get; set; }
    }
}
