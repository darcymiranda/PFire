using System.Threading.Tasks;
using PFire.Core.Session;

/* 
 * Packet 44 - Accept the Clan Request
 * Based on clan id, the user accepts the request.
 * Then process all of it.
 * 
 */

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal class ClanRequestAccept : XFireMessage
    {
        public ClanRequestAccept() : base(XFireMessageType.ClanRequestAccept) { }

        [XMessageField(0x6c)]
        public string ClanId { get; set; }

        public async override Task Process(IXFireClient context)
        {
            //TODO: Remove request 
            //      Add user to clan as normal ranked
            //      Send UserClans back
        }
    }
}
