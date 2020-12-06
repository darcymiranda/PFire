using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class UserLookupResult : XFireMessage
    {
        private readonly string _queryByUsername;

        public UserLookupResult(string username) : base(XFireMessageType.UserLookupResult)
        {
            _queryByUsername = username;

            Usernames = new List<string>();
            FirstNames = new List<string>();
            LastNames = new List<string>();
            Emails = new List<string>();
        }

        [XMessageField("name")]
        public List<string> Usernames { get; }

        [XMessageField("fname")]
        public List<string> FirstNames { get; }

        [XMessageField("lname")]
        public List<string> LastNames { get; }

        [XMessageField("email")]
        public List<string> Emails { get; }

        public override async Task Process(IXFireClient context)
        {
            var queryUsers = await context.Server.Database.QueryUsers(_queryByUsername);
            var usernames = queryUsers.Select(a => a.Username).ToList();

            Usernames.AddRange(usernames);

            // Don't really care about these but they're necessary to work properly
            var unknowns = usernames.Select(a => "Unknown").ToList();

            FirstNames.AddRange(unknowns);
            LastNames.AddRange(unknowns);
            Emails.AddRange(unknowns);
        }
    }
}
