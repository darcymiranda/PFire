using System.Collections.Generic;

/* 
 * Packet 176 - Friend Clan Info
 * List all the clans that your friend belongs to.
 * See UserClans for the general breakdown of variables.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendClanInfo : XFireMessage
    {
        public FriendClanInfo(int userId) : base(XFireMessageType.ClanFriendClanInfo)
        {
            UserId = userId;
        }
        [XMessageField(0x01)]
        public int UserId { get; set; }
        [XMessageField(0x6c)]
        public List<int> ClanIds { get; set; } = new List<int>();
        [XMessageField(0x72)]
        public List<string> ClanShortName { get; set; } = new List<string>();
        [XMessageField(0x81)]
        public List<string> ClanLongName { get; set; } = new List<string>();
        [XMessageField(0x6d)]
        public List<string> NameWithinClan { get; set; } = new List<string>();
    }
}
