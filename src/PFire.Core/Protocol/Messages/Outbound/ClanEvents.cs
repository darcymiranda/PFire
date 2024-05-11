using System.Collections.Generic;

/* 
 * Packet 170 - Clan Events
 * Put in the clan ID that is running the event.
 * Event ID is unique globally.
 * Game ID is the game id for the event.
 * Event Name is the actual event name.
 * Start and End Time is the times which the event begins and ends.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ClanEvents : XFireMessage
    {
        public ClanEvents(int clanId) : base(XFireMessageType.ClanEvents)
        {
        }

        [XMessageField(0x6c)]
        public List<int> ClanId { get; set; } = new List<int>();
        [XMessageField(0x8b)]
        public List<int> EventId { get; set; } = new List<int>();
        [XMessageField(0x21)]
        public List<int> GameId { get; set; } = new List<int>();
        [XMessageField(0x05)]
        public List<string> EventName { get; set; } = new List<string>();
        [XMessageField(0x50)]
        public List<int> StartTime { get; set; } = new List<int>();
        [XMessageField(0x8c)]
        public List<int> EndTime { get; set; } = new List<int>();
    }
}
