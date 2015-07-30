using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class UserLookupResult : IMessage
    {
        [XFireAttributeDef("name")]
        public List<string> Usernames { get; private set; }

        [XFireAttributeDef("fname")]
        public List<string> FirstNames { get; private set; }

        [XFireAttributeDef("lname")]
        public List<string> LastNames { get; private set; }

        [XFireAttributeDef("email")]
        public List<string> Emails { get; private set; }

        public short MessageTypeId
        {
            get { return 143; }
        }

        private string queryByUsername;

        public UserLookupResult(string username)
        {
            queryByUsername = username;
            Usernames = new List<string>();
            FirstNames = new List<string>();
            LastNames = new List<string>();
            Emails = new List<string>();
        }

        public void Process(Context context)
        {
            var users = context.Server.Database.QueryUsers(queryByUsername);
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
