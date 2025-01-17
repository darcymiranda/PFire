using PFire.Infrastructure.Entities;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class GroupCreateConfirmation : XFireMessage
    {
        public GroupCreateConfirmation(UserGroup group) : base(XFireMessageType.GroupCreateConfirmation)
        {
            GroupId = group.Id;
            GroupName = group.Name;
        }

        [XMessageField(0x19)]
        public int GroupId { get; set; }

        [XMessageField(0x1A)]
        public string GroupName { get; set; }
    }
}
