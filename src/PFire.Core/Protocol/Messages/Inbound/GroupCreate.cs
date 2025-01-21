using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal class GroupCreate : XFireMessage
    {
        public GroupCreate() : base(XFireMessageType.GroupCreate) { }

        [XMessageField(0x1A)]
        public string Name { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var group = await context.Server.Database.CreateGroup(context.User, Name);
            if(group is not null)
            {
                await context.SendAndProcessMessage(new GroupCreateConfirmation(group));
            }
        }
    }
}
