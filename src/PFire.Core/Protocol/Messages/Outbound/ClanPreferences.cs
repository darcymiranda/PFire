using System.Collections.Generic;

/* 
 * Packet 178 - Clan Preferences
 * Though this shares the same attribute keys of some clan packets. I don't know what it wants or needs.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanPreferences : XFireMessage
    {
        public ClanPreferences() : base(XFireMessageType.ClanPreferences)
        {
        }
        [XMessageField(0x6c)]
        public int ClanId { get; set; }
        [XMessageField(0x4c)]
        public List<int> Unk4c { get; set; } = new List<int>(); //Shares the same key as user preferences?
        [XMessageField(0x2a)]
        public List<string> Unk2a { get; set; } = new List<string>(); //Shares the same key as Rank titles?
    }
}
