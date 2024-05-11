using PFire.Core.Models;
using PFire.Core.Session;
using PFire.Infrastructure.Entities;

/* 
 Base Avatar URLs:

avatarType: 0
Xfire Default Logo

avatarType: 1
 "http://media.xfire.com/xfire/xf/images/avatars/gallery/default/%03u.gif" // avatar id

 avatarType: 2
 "http://screenshot.xfire.com/avatar/100/%s.jpg?%u" // username, revision number 
*/

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendAvatarInfo : XFireMessage
    {
        public FriendAvatarInfo(UserModel user) : base(XFireMessageType.UserAvatarInfo)
        {
            //TODO: No functionality for now, needs a web frontend.
            UserId = user.Id;
            AvatarType = 0;
            AvatarRevision = 0;
        }

        [XMessageField(0x01)]
        public int UserId { get; set; }

        [XMessageField(0x34)]
        public int AvatarType { get; set; }

        [XMessageField(0x1F)]
        public int AvatarRevision { get; set; }




    }
}
