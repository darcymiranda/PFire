using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal class GroupRename : XFireMessage
    {
        public GroupRename() : base(XFireMessageType.GroupRename) { }

        [XMessageField(0x1A)]
        public string Name { get; set; }

        [XMessageField(0x19)]
        public int GroupId { get; set; }

        public override async Task Process(IXFireClient context)
        {
            await context.Server.Database.RenameUserGroup(GroupId, Name);
        }
    }
}
