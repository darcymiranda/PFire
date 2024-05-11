/* 
 * Packet 189 - Clan Game Server Unknown
 * This one kind of sort of sorts servers based on server ids? 
 * In my observations, it will highlight the server that matches the id... sometimes.
 * If it doesn't match, it will show all game servers.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal class ClanGameServerUnknown : XFireMessage
    {
        public ClanGameServerUnknown(int clanId, int serverId) : base(XFireMessageType.ClanGameServerUnknown)
        {
            ClanId = clanId;
            ServerId = serverId;
        }
        [XMessageField(0x6c)]
        public int ClanId { get; set; }
        [XMessageField(0x0b)]
        public int ServerId { get; set; }
    }
}
