using System.Collections.Generic;
using System.Linq;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    public sealed  class UserLookupResult : XFireMessage
    {
        private readonly string _queryByUsername;

        public UserLookupResult(string username)
            : base(XFireMessageType.UserLookupResult)
        {
            _queryByUsername = username;

            Usernames = new List<string>();
            FirstNames = new List<string>();
            LastNames = new List<string>();
            Emails = new List<string>();
        }

        [XMessageField("name")]
        public List<string> Usernames { get; private set; }

        [XMessageField("fname")]
        public List<string> FirstNames { get; private set; }

        [XMessageField("lname")]
        public List<string> LastNames { get; private set; }

        [XMessageField("email")]
        public List<string> Emails { get; private set; }

        public override void Process(XFireClient context)
        {
            var users = context.Server.Database.QueryUsers(_queryByUsername);
            var usernames = users.Select(a => a.Username);

            Usernames.AddRange(usernames);

            // Don't really care about these but they're necessary to work properly
            var unknowns = usernames.Select(a => "Unknown");

            FirstNames.AddRange(unknowns);
            LastNames.AddRange(unknowns);
            Emails.AddRange(unknowns);
        }
    }
}
