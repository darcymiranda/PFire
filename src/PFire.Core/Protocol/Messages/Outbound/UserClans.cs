using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol;
using System.Collections.Generic;

/* 
 * Packet 158 - User Clans
 * This is a list pulled from the server that are remote clans you are in.
 * 
 * Xfire has "Clan types" which I think change the verbage (and probably functions on the site)
 * I identified them as such:
 * 0 = clan, 1 = guild, 2 = team, 3 = squad, 4 = league, 5 = cult, 6 = corporation, 7 = club, 8 = group
 * 
 * !!! Having a clan id of 0 seems to make things not work in the advanced infoview panel?
 * 
 * ClanName is the long clan name shown in the client.
 * ClanShortName is the shortened name that is put into urls and other misc stuff.
 * 
 * Unknown 0xB0 is really unknown in what it does. I tried all sorts of numbers but theres no visual change.
 */

internal sealed class UserClans : XFireMessage
{
    public UserClans() : base(XFireMessageType.UserClans)
    { 
    }

    [XMessageField(0x6c)]
    public List<int> ClanId { get; set; } = new();
    [XMessageField(0x02)]
    public List<string> ClanName { get; set; } = new();
    [XMessageField(0x72)]
    public List<string> ClanShortName { get; set; } = new();
    [XMessageField(0x34)]
    public List<int> ClanType { get; set; } = new();
    [XMessageField(0xb0)]
    public List<int> UnkB0 { get; set; } = new();
}
