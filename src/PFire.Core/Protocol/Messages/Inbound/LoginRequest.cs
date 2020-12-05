using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class LoginRequest : XFireMessage
    {
        public LoginRequest() : base(XFireMessageType.LoginRequest) {}

        [XMessageField("name")]
        public string Username { get; private set; }

        [XMessageField("password")]
        public string Password { get; private set; }

        [XMessageField("flags")]
        public int Flags { get; private set; }

        public override void Process(IXFireClient context)
        {
            var user = context.Server.Database.QueryUser(Username);
            if (user != null)
            {
                if (!BCrypt.Net.BCrypt.Verify(Password, user.Password))
                {
                    context.SendAndProcessMessage(new LoginFailure());
                    return;
                }
            }
            else
            {
                var hashPassword = BCrypt.Net.BCrypt.HashPassword(Password);
                user = context.Server.Database.InsertUser(Username, hashPassword, context.Salt);
            }

            // Remove any older sessions from this user (duplicate logins)
            context.RemoveDuplicatedSessions(user);

            context.User = user;

            var success = new LoginSuccess();
            context.SendAndProcessMessage(success);
        }
    }
}
