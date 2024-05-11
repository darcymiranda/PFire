using System.Collections.Generic;

/* 
 * Packet 168 - Clan Rank Permissions
 * This packet would change the permissions of each rank.
 * But I have no visual reference to see what had changed. So, I am not sure what values work.
 * Maybe this is for chatrooms?
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal class ClanRankPermissions : XFireMessage
    {
        public ClanRankPermissions() : base(XFireMessageType.ClanRankPermissions)
        {
        }
        [XMessageField(0x6c)]
        public int ClanId { get; set; }
        [XMessageField(0x74)]
        public List<int> RankId { get; set; } = new List<int>();
        [XMessageField(0x34)]
        public List<int> Unk34 { get; set; } = new List<int>();
        [XMessageField(0x2a)]
        public List<string> Unk2a { get; set; } = new List<string>();
    }
}
