using PFire.Core.Entities;
using PFire.Core.Models;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class UserRequestAdvancedInfo : XFireMessage
    {
        public UserRequestAdvancedInfo() : base(XFireMessageType.UserRequestAdvancedInfo) { }

        [XMessageField(0x01)]
        public int UserId { get; set; }

        public override async Task Process(IXFireClient context)
        {
            // Per MacFire documentation, they observed that this request send packets:
            //
            // 172 Friend's screenshots info
            // 174 Friend's avatar info
            // 182 Friend's videos
            // 176 Friend's clan membership info

            UserModel user = await context.Server.Database.QueryUser(UserId);

            if (user != null)
            {
                List<Screenshot> friendScreenshots = new List<Screenshot>();

                await context.SendAndProcessMessage(new FriendScreenshots(user.Id, friendScreenshots)); //172
                await context.SendAndProcessMessage(new FriendAvatarInfo(user)); //174
                await context.SendAndProcessMessage(new FriendVideos(user.Id)); //182
                await context.SendAndProcessMessage(new FriendClanInfo(user.Id));
            }
            
            
        }
    }
}
