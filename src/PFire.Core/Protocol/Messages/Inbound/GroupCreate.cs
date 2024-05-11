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
            var newGroup = await context.Server.Database.CreateUserGroup(context.User, Name);

            if (newGroup != null)
            {
                await context.SendAndProcessMessage(new GroupCreateConfirmation(newGroup));
            }
        }
    }
}
