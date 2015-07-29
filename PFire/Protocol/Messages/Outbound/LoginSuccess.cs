using PFire.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class LoginSuccess : IMessage
    {
        [XFireAttributeDef("userid")]
        public int UserId { get; private set; }

        [XFireAttributeDef("sid")]
        public Guid SessionId { get; private set; }

        [XFireAttributeDef("nick")]
        public string Nickname { get; private set; }

        [XFireAttributeDef("status")]
        public int Status { get; private set; }

        [XFireAttributeDef("dlset")]
        public string DlSet { get; private set; }

        [XFireAttributeDef("p2pset")]
        public string P2PSet { get; private set; }

        [XFireAttributeDef("clntset")]
        public string ClientSet { get; private set; }

        [XFireAttributeDef("minrect")]
        public int MinRect { get; private set; }

        [XFireAttributeDef("maxrect")]
        public int MaxRect { get; private set; }

        [XFireAttributeDef("ctry")]
        public int Country { get; private set; }

        [XFireAttributeDef("n1")]
        public int N1 { get; private set; }

        [XFireAttributeDef("n2")]
        public int N2 { get; private set; }

        [XFireAttributeDef("n3")]
        public int N3 { get; private set; }

        [XFireAttributeDef("pip")]
        public int PublicIp { get; private set; }

        [XFireAttributeDef("salt")]
        public string Salt { get; private set; }

        [XFireAttributeDef("reason")]
        public string Reason { get; private set; }

        public short MessageTypeId
        {
            get { return 130; }
        }

        public void Process(Context context)
        {
            UserId = context.User.UserId;
            SessionId = context.SessionId;
            Status = 0;//BitConverter.ToInt32(new byte[] { 0x7f, 0x05, 0x0, 0x0 }, 0);
            Nickname = String.IsNullOrEmpty(context.User.Nickname) ? context.User.Username : context.User.Nickname;
            MinRect = 1;
            MaxRect = 164867;
            var ipAddress = StripPortFromIPAddress(context.TcpClient.Client.RemoteEndPoint.ToString());
            PublicIp = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            Salt = context.Salt;
            Reason = "Mq_P8Ad3aMEUvFinw0ceu6FITnZTWXxg46XU8xHW";

            Debug.WriteLine("User {0}[{1}] logged in successfully with session id {2}", context.User.Username, context.User.UserId, context.SessionId);
            Console.WriteLine("User {0} logged in", context.User.Username);

            //N1 = BitConverter.ToInt32(new byte[] { 0x3c, 0xb2, 0x58, 0xd0 }, 0);
            //N2 = BitConverter.ToInt32(new byte[] { 0x3d, 0xb2, 0x58, 0xd0 }, 0);
            //N3 = BitConverter.ToInt32(new byte[] { 0x3e, 0xb2, 0x58, 0xd0 }, 0);
        }

        private string StripPortFromIPAddress(string address)
        {
            return address.Substring(0, address.IndexOf(":"));
        }
    }
}
