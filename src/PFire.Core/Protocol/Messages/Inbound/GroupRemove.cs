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
            await context.Server.Database.RemoveGroup(context.User.Id, GroupId);
        }
    }
}
