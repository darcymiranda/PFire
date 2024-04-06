using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal class GroupRemove : XFireMessage
    {
        public GroupRemove() : base(XFireMessageType.GroupRemove) { }

        [XMessageField(0x19)]
        public int GroupId { get; set; }

        public override async Task Process(IXFireClient context)
        {
            //TODO: This should be checked if the user owns the group.
            await context.Server.Database.RemoveUserGroup(GroupId);
        }
    }
}
