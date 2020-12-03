using System.Collections.Generic;
using System.Linq;
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

        public override void Process(IXFireClient context)
        {
            var usernames = context.Server.Database.QueryUsers(_queryByUsername).Select(a => a.Username).ToList();

            Usernames.AddRange(usernames);

            // Don't really care about these but they're necessary to work properly
            var unknowns = usernames.Select(a => "Unknown").ToList();

            FirstNames.AddRange(unknowns);
            LastNames.AddRange(unknowns);
            Emails.AddRange(unknowns);
        }
    }
}
