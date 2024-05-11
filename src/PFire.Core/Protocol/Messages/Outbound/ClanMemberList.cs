using System.Collections.Generic;

/* 
 * Packet 159 - Clan Member List
 * Should be sent after the User Clans or any querying clan stuff. 
 * 
 * Defines each user, interestingly will overwrite any user data when you logged in, if the user ids match.
 * 
 * Unknown 74 MIGHT be rank ids, but I have no visual evidence for this.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanMemberList : XFireMessage
    {
        public ClanMemberList(int clanId) : base(XFireMessageType.ClanMemberList)
        {
            ClanId = clanId;
        }

        [XMessageField(0x6c)]
        public int ClanId { get; set; }
        [XMessageField(0x01)]
        public List<int> UserId { get; set; } = new List<int>();
        [XMessageField(0x02)]
        public List<string> Username { get; set; } = new List<string>();
        [XMessageField(0x0d)]
        public List<string> Nickname { get; set; } = new List<string>();
        [XMessageField(0x6d)]
        public List<string> NameWithinClan { get; set; } = new List<string>();
        [XMessageField(0x74)]
        public List<int> Unk74 { get; set; } = new List<int>();
    }
}
