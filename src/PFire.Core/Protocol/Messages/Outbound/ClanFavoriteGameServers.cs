using System.Collections.Generic;

/* 
 * Packet 188 - Clan Favorite Game Servers
 * A list of favorite game servers. No querying is done, so its just the information.
 * 
 * FavoriteId changes the sorting. All the game info is as usual.
 * 
 * Unknown 36 has no visual effect, it doesn't modify the url in the client nor does it use it for sorting.
 * 
 */
namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanFavoriteGameServers : XFireMessage
    {
        public ClanFavoriteGameServers(int clanId) : base(XFireMessageType.ClanFavoriteGameServers)
        {
        }

        [XMessageField(0x6c)]
        public List<int> ClanId { get; set; } = new List<int>();
        [XMessageField(0x0b)]
        public List<int> FavoriteId { get; set; } = new List<int>();
        [XMessageField(0x21)]
        public List<int> GameId { get; set; } = new List<int>();
        [XMessageField(0x22)]
        public List<int> GameIp { get; set; } = new List<int>();
        [XMessageField(0x23)]
        public List<int> GamePort { get; set; } = new List<int>();
        [XMessageField(0x02)]
        public List<string> ServerName { get; set; } = new List<string>();
        [XMessageField(0x36)]
        public List<int> Unk36 { get; set; } = new List<int>();
    }
}
