using System.Collections.Generic;

/* 
 * Packet 177 - Clan News Posted
 * News ID might be globally unique like event ids are.
 * News Title is what is shown in the client, like an article name.
 * News Date is Unix Time like usual.
 * Author Username and Nickname can be anything, but the Username is the profile username when clicked.
 * Author Clan Username is what is shown for a name.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanNewsPostedOnPage : XFireMessage
    {
        public ClanNewsPostedOnPage(int clanId) : base(XFireMessageType.ClanNewsPostedOnPage)
        {
        }

        [XMessageField(0x6c)]
        public List<int> ClanId { get; set; } = new List<int>();
        [XMessageField(0x91)]
        public List<int> NewsId { get; set; } = new List<int>();
        [XMessageField(0x54)]
        public List<string> NewsTitle { get; set; } = new List<string>();
        [XMessageField(0x50)]
        public List<int> NewsDate { get; set; } = new List<int>();
        [XMessageField(0x02)]
        public List<string> AuthorUsername { get; set; } = new List<string>();
        [XMessageField(0x0d)]
        public List<string> AuthorNickname { get; set; } = new List<string>();
        [XMessageField(0x6d)]
        public List<string> AuthorClanUsername { get; set; } = new List<string>();
    }
}
