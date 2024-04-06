using System;
using System.Collections.Generic;
using PFire.Core.Session;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class GroupsFriends : XFireMessage
    {
        public GroupsFriends() : base(XFireMessageType.GroupsFriends)
        {
            UserIds = new List<int>();
            GroupIds = new List<int>();
        }

        [XMessageField(0x01)]
        public List<int> UserIds { get; }

        [XMessageField(0x19)]
        public List<int> GroupIds { get; }

        public override async Task Process(IXFireClient context)
        {
            var result = await context.Server.Database.GetAllUserGroups(context.User);

            foreach (var group in result)
            {
                var memberRoster = group.MemberIds;
                if (memberRoster != null)
                {
                    foreach (var member in memberRoster)
                    {
                        GroupIds.Add(group.Id);
                        UserIds.Add(member);
                    }
                }
            }
        }
    }
}
