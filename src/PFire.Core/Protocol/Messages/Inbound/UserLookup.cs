using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class UserLookup : XFireMessage
    {
        public UserLookup() : base(XFireMessageType.UserLookup) {}

        [XMessageField("name")]
        public string Username { get; set; }

        [XMessageField("fname")]
        public string FirstName { get; set; }

        [XMessageField("lname")]
        public string LastName { get; set; }

        [XMessageField("email")]
        public string Email { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var result = new UserLookupResult(Username);
            await context.SendAndProcessMessage(result);
        }
    }
}
