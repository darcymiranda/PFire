using System.Collections.Generic;

/* 
 * Packet 173 - User Advanced Info Changed
 * This just forces the client to send back UserRequestAdvancedInfo immediately based on user id. 
 * Must be called when a user uploads a video or changes their avatar.
 * 
 */

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class UserAdvancedInfoChanged : XFireMessage
    {
        public UserAdvancedInfoChanged(int userId) : base(XFireMessageType.UserAdvancedInfoChanged)
        {
            UserId = userId;
        }

        [XMessageField(0x01)]
        public int UserId { get; set; }

    }
}
