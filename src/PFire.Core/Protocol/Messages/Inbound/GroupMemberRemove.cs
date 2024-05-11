using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal class GroupMemberRemove : XFireMessage
    {
        public GroupMemberRemove() : base(XFireMessageType.GroupMemberRemove) { }

        [XMessageField(0x01)]
        public int UserId { get; set; }

        [XMessageField(0x19)]
        public int GroupId { get; set; }

        public override async Task Process(IXFireClient context)
        {
            await context.Server.Database.RemoveMemberFromUserGroup(GroupId, UserId);
        }
    }
}
