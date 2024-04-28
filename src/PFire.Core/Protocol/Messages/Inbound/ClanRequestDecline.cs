using System.Threading.Tasks;
using PFire.Core.Session;

/* 
 * Packet 45 - Decline the Clan Request
 * Based on clan id, the user declines the request.
 * Then process all of it.
 * 
 */

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal class ClanRequestDecline : XFireMessage
    {
        public ClanRequestDecline() : base(XFireMessageType.ClanRequestDecline) { }

        [XMessageField(0x6c)]
        public string ClanId { get; set; }

        public async override Task Process(IXFireClient context)
        {
            //TODO: Remove the request from the database so the user doesn't get it again on log in.
        }
    }
}
