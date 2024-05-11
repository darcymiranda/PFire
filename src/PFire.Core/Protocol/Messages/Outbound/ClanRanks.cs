using System.Collections.Generic;

/* 
 * Packet 167 - Clan Ranks
 * This defines the ranks within the clan. 
 * I couldn't get any visual feedback, but I wonder if these rank ids and such are used for something else (maybe chatrooms?)
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanRanks : XFireMessage
    {
        public ClanRanks() : base(XFireMessageType.ClanRanks)
        {
        }
        [XMessageField(0x6c)]
        public int ClanId { get; set; }
        [XMessageField(0x74)]
        public List<int> RankId { get; set; } = new List<int>();
        [XMessageField(0x2a)]
        public List<string> RankTitle { get; set; } = new List<string>();
    }
}
