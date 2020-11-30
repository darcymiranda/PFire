﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol;
using PFire.Protocol.Messages;
using System.Net.Sockets;
using PFire.Database;
using PFire.Core.Protocol.Messages;
using PFire.Core.Util;

namespace PFire.Session
{
    public sealed class XFireClient : Disposable
    {
        public PFireServer Server { get; set; }

        public User User { get; set; }

        public bool Initialized { get; private set; }
        public string Salt { get; private set; }
        public Guid SessionId { get; private set; }

        public TcpClient TcpClient { get; private set; }

        public XFireClient(TcpClient tcpClient)
        {
            TcpClient = tcpClient;

            // TODO: be able to use unique salts
            Salt = "4dc383ea21bf4bca83ea5040cb10da62";//Guid.NewGuid().ToString().Replace("-", string.Empty);
            SessionId = Guid.NewGuid();
        }

        public void InitializeClient()
        {
            Initialized = true;
        }
        public void SendAndProcessMessage(XFireMessage message)
        {
            message.Process(this);
            SendMessage(message);
        }

        public void SendMessage(XFireMessage message)
        {
            var payload = MessageSerializer.Serialize(message);
            TcpClient.Client.Send(payload);
            Console.WriteLine("Sent message[{0},{1}]: {2}",
                User != null ? User.Username : "unknown",
                User != null ? User.UserId : -1,
                message);
        }

        public void TestSend(byte[] data)
        {
            Debug.WriteLine("Sent message to client {0}: {1}", TcpClient.Client.RemoteEndPoint, BitConverter.ToString(data));
            TcpClient.Client.Send(data);
        }
    }
}
