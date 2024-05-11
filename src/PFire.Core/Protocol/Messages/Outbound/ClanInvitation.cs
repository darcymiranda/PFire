using System.Collections.Generic;

/* 
 * Packet 165 - Clan Invitation
 * 
 * Clan ID will be the clan ID associated.
 * LongName and Shortname is the Long and Short names of the clan. Shortname is shown in the invitation to the user and used in urls, 
 * but the long name is used otherwise.
 * Username/Nickname is the names for the inviter.
 * Clan Type is the clan type which shows in the invite. 
 * 0 = clan, 1 = guild, 2 = team, 3 = squad, 4 = league, 5 = cult, 6 = corporation, 7 = club, 8 = group
 * Message is the invitation message.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanInvitation: XFireMessage
    {
        public ClanInvitation(int userId) : base(XFireMessageType.ClanInvitation)
        {
        }
        [XMessageField(0x6c)]
        public List<int> ClanIds { get; set; } = new List<int>();
        [XMessageField(0x72)]
        public List<string> LongName { get; set; } = new List<string>();
        [XMessageField(0x81)]
        public List<string> ShortName { get; set; } = new List<string>();
        [XMessageField(0x34)]
        public List<int> ClanType { get; set; } = new List<int>();
        [XMessageField(0x02)]
        public List<string> Username { get; set; } = new List<string>();
        [XMessageField(0x0d)]
        public List<string> Nickname { get; set; } = new List<string>();
        [XMessageField(0x2e)]
        public List<string> Message { get; set; } = new List<string>();
    }
}
