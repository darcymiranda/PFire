using System;
using System.Diagnostics;
using System.Net;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    public sealed class LoginSuccess : XFireMessage
    {
        public LoginSuccess() : base(XFireMessageType.LoginSuccess) {  }

        [XMessageField("userid")]
        public int UserId { get; private set; }

        [XMessageField("sid")]
        public Guid SessionId { get; private set; }

        [XMessageField("nick")]
        public string Nickname { get; private set; }

        [XMessageField("status")]
        public int Status { get; private set; }

        [XMessageField("dlset")]
        public string DlSet { get; private set; }

        [XMessageField("p2pset")]
        public string P2PSet { get; private set; }

        [XMessageField("clntset")]
        public string ClientSet { get; private set; }

        [XMessageField("minrect")]
        public int MinRect { get; private set; }

        [XMessageField("maxrect")]
        public int MaxRect { get; private set; }

        [XMessageField("ctry")]
        public int Country { get; private set; }

        [XMessageField("n1")]
        public int N1 { get; private set; }

        [XMessageField("n2")]
        public int N2 { get; private set; }

        [XMessageField("n3")]
        public int N3 { get; private set; }

        [XMessageField("pip")]
        public int PublicIp { get; private set; }

        [XMessageField("salt")]
        public string Salt { get; private set; }

        [XMessageField("reason")]
        public string Reason { get; private set; }

        public override void Process(XFireClient context)
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

            Debug.WriteLine("User {0}[{1}] logged in successfully with session id {2}", context.User.Username, context.User.UserId, context.SessionId);
            Console.WriteLine("User {0} logged in", context.User.Username);
        }

        private static string StripPortFromIPAddress(string address)
        {
            return address.Substring(0, address.IndexOf(":"));
        }
    }
}
