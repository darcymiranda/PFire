using System.Threading.Tasks;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class LoginRequest : XFireMessage
    {
        public LoginRequest() : base(XFireMessageType.LoginRequest) {}

        [XMessageField("name")]
        public string Username { get; set; }

        [XMessageField("password")]
        public string Password { get; set; }

        [XMessageField("flags")]
        public int Flags { get; set; }

        public override async Task Process(IXFireClient context)
        {
            var user = await context.Server.Database.QueryUser(Username);
            if (user != null)
            {
                if (user.Password != Password)
                {
                    await context.SendAndProcessMessage(new LoginFailure());

                    return;
                }
            }
            else
            {
                user = await context.Server.Database.InsertUser(Username, Password, context.Salt);
            }

            // Remove any older sessions from this user (duplicate logins)
            context.RemoveDuplicatedSessions(user);

            context.User = user;

            var success = new LoginSuccess();
            await context.SendAndProcessMessage(success);
        }
    }
}
