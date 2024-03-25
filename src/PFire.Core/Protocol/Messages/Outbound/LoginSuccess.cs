﻿using System;
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
        public uint UserId { get; set; }

        [XMessageField("sid")]
        public Guid SessionId { get; set; }

        [XMessageField("nick")]
        public string Nickname { get; set; }

        [XMessageField("status")]
        public uint Status { get; set; }

        [XMessageField("dlset")]
        public string DlSet { get; set; }

        [XMessageField("p2pset")]
        public string P2PSet { get; set; }

        [XMessageField("clntset")]
        public string ClientSet { get; set; }

        [XMessageField("minrect")]
        public uint MinRect { get; set; }

        [XMessageField("maxrect")]
        public uint MaxRect { get; set; }

        [XMessageField("ctry")]
        public uint Country { get; set; }

        [XMessageField("n1")]
        public uint N1 { get; set; }

        [XMessageField("n2")]
        public uint N2 { get; set; }

        [XMessageField("n3")]
        public uint N3 { get; set; }

        [XMessageField("pip")]
        public uint PublicIp { get; set; }

        [XMessageField("salt")]
        public string Salt { get; set; }

        [XMessageField("reason")]
        public string Reason { get; set; }

        public override Task Process(IXFireClient context)
        {
            UserId = context.User.Id;
            SessionId = context.SessionId;
            Status = 0;
            Nickname = string.IsNullOrEmpty(context.User.Nickname) ? context.User.Username : context.User.Nickname;
            MinRect = 1;
            MaxRect = 164867;
            var ipAddress = StripPortFromIpAddress(context.RemoteEndPoint.ToString());
            PublicIp = BitConverter.ToUInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            Salt = context.Salt;
            Reason = "Mq_P8Ad3aMEUvFinw0ceu6FITnZTWXxg46XU8xHW";

            context.Logger.LogDebug($"User {context.User.Username}[{context.User.Id}] logged in successfully with session id {context.SessionId}");
            context.Logger.LogInformation($"User {context.User.Username} logged in");

            return Task.CompletedTask;
        }

        private static string StripPortFromIpAddress(string address)
        {
            return address.Substring(0, address.IndexOf(":", StringComparison.Ordinal));
        }
    }
}
