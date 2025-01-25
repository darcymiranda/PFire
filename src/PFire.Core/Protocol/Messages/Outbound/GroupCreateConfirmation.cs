using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal class GroupCreateConfirmation : XFireMessage
    {
        public GroupCreateConfirmation(GroupModel group) : base(XFireMessageType.GroupCreateConfirmation) 
        { 
            Id = group.Id;
            Name = group.Name;
        }

        [XMessageField(0x19)]
        public int Id { get; set; }

        [XMessageField(0x1A)]
        public string Name { get; set; }

        public override Task Process(IXFireClient context)
        {
            //nothing to see here bud
            return Task.CompletedTask;
        }
    }
}
