using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class LoginSuccess : XFireMessage
    {
        public LoginSuccess() : base(XFireMessageType.LoginSuccess) {}

        [XMessageField("userid")]
        public int UserId { get; set; }

        [XMessageField("sid")]
        public Guid SessionId { get; set; }

        [XMessageField("nick")]
        public string Nickname { get; set; }

        [XMessageField("status")]
        public int Status { get; set; }

        [XMessageField("dlset")]
        public string DlSet { get; set; }

        [XMessageField("p2pset")]
        public string P2PSet { get; set; }

        [XMessageField("clntset")]
        public string ClientSet { get; set; }

        [XMessageField("minrect")]
        public int MinRect { get; set; }

        [XMessageField("maxrect")]
        public int MaxRect { get; set; }

        [XMessageField("ctry")]
        public int Country { get; set; }

        [XMessageField("n1")]
        public int N1 { get; set; }

        [XMessageField("n2")]
        public int N2 { get; set; }

        [XMessageField("n3")]
        public int N3 { get; set; }

        [XMessageField("pip")]
        public int PublicIp { get; set; }

        [XMessageField("salt")]
        public string Salt { get; set; }

        [XMessageField("reason")]
        public string Reason { get; set; }

        public override Task Process(IXFireClient context)
        {
            UserId = context.User.UserId;
            SessionId = context.SessionId;
            Status = 0;
            Nickname = string.IsNullOrEmpty(context.User.Nickname) ? context.User.Username : context.User.Nickname;
            MinRect = 1;
            MaxRect = 164867;
            var ipAddress = StripPortFromIPAddress(context.RemoteEndPoint.ToString());
            PublicIp = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            Salt = context.Salt;
            Reason = "Mq_P8Ad3aMEUvFinw0ceu6FITnZTWXxg46XU8xHW";

            context.Logger.LogDebug($"User {context.User.Username}[{context.User.UserId}] logged in successfully with session id {context.SessionId}");
            context.Logger.LogInformation($"User {context.User.Username} logged in");

            return Task.CompletedTask;
        }

        private static string StripPortFromIPAddress(string address)
        {
            return address.Substring(0, address.IndexOf(":"));
        }
    }
}
