/* 
 * Packet 164 - Clan Name Changed
 * Self-explainatory. The clan name has been changed.
 * 
 * ClanId is the Clan ID.
 * NewClanName is the new clan name.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanNameChanged : XFireMessage
    {
        public ClanNameChanged(int clanId, string newName) : base(XFireMessageType.ClanNameChanged)
        {
            ClanId = clanId;
            NewClanName = newName;
        }
        [XMessageField(0x6c)]
        public int ClanId { get; set; }
        [XMessageField(0x02)]
        public string NewClanName { get; set; }
    }
}
