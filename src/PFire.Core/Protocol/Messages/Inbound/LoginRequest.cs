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

        public override void Process(XFireClient context)
        {
            var user = context.Server.Database.QueryUser(Username);
            if (user != null)
            {
                if (user.Password != Password)
                {
                    context.SendAndProcessMessage(new LoginFailure());
                    return;
                }
            }
            else
            {
                user = context.Server.Database.InsertUser(Username, Password, context.Salt);
            }

            // Remove any older sessions from this user (duplicate logins)
            context.RemoveDuplicatedSessions(user);

            context.User = user;

            var success = new LoginSuccess();
            context.SendAndProcessMessage(success);
        }
    }
}
