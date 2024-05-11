/* 
 * Packet 171 - Clan Event Deleted
 * Deletes the event based by event id. (Which implies that the event ids are globally different and not per clan)
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanEventDeleted : XFireMessage
    {
        public ClanEventDeleted(int eventId) : base(XFireMessageType.ClanEventDeleted)
        {
            EventId = eventId;
        }

        [XMessageField(0x8b)]
        public int EventId { get; set; }
    }
}
