/* 
 * Packet 178 - Clan Avatar Revision
 * This affects the v= in the clan avatar url. !!! THERE MIGHT BE OTHER USES BUT THIS IS THE ONLY THING IVE SEEN THUS FAR.
 * Full request URI: http://screenshot.xfire.com/clan_logo/##/CLANSHORTNAME.jpg?v=VERSION
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal class ClanAvatarVersion : XFireMessage
    {
        public ClanAvatarVersion(int clanId, int version) : base(XFireMessageType.ClanAvatarVersion)
        {
            ClanId = clanId;
            Version = version;
        }
        [XMessageField(0x6c)]
        public int ClanId { get; set; }
        [XMessageField(0x92)]
        public int Version { get; set; }
    }
}
