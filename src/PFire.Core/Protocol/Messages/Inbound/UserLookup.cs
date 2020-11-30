﻿using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    public sealed class UserLookup : XFireMessage
    {
        public UserLookup() : base(XFireMessageType.UserLookup) { }

        [XMessageField("name")]
        public string Username { get; private set; }

        [XMessageField("fname")]
        public string FirstName { get; private set; }

        [XMessageField("lname")]
        public string LastName { get; private set; }

        [XMessageField("email")]
        public string Email { get; private set; }

        public override void Process(XFireClient context)
        {
            var result = new UserLookupResult(Username);
            context.SendAndProcessMessage(result);
        }
    }
}
