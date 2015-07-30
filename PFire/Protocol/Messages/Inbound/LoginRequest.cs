using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;

namespace PFire.Protocol.Messages.Inbound
{
    public class LoginRequest : IMessage
    {
        [XFireAttributeDef("name")]
        public string Username { get; private set; }

        [XFireAttributeDef("password")]
        public string Password { get; private set; }

        [XFireAttributeDef("flags")]
        public int Flags { get; private set; }

        public short MessageTypeId
        {
            get { return 1; }
        }

        public void Process(Context context)
        {
            var user = context.Server.Database.QueryUser(Username);
            if (user != null)
            {
                if (user.Password != Password)
                {
                    var failure = new LoginFailure();
                    failure.Process(context);
                    context.SendMessage(failure);
                    return;
                }
            }
            else
            {
                user = context.Server.Database.InsertUser(Username, Password, context.Salt);
            }

            // Remove any older sessions from this user (duplicate logins)
            var otherSession = context.Server.GetSession(user);
            if (otherSession != null)
            {
                context.Server.RemoveSession(otherSession);
                otherSession.TcpClient.Close();
            }

            context.User = user;

            var success = new LoginSuccess();
            success.Process(context);
            context.SendMessage(success);
        }
    }
}
