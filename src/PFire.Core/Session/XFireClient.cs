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
using System.Net;
using PFire.Protocol.Messages.Inbound;
using PFire.Protocol.Messages.Outbound;

namespace PFire.Session
{
    public class XFireClient
    {
        public PFireServer Server {
            get;
            set;
        }

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
            Initialized = true;
        }

        public void InitializeClient()
        {
            // Does nothing, kept for compatibility reasons.
            //Initialized = true;
        }

        public void InitializeClient(PFireServer server)
        {
            this.Server = server;
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
            Console.WriteLine("[XFireClient] Sent message[{0},{1}]: {2}. Remote Endpoint: {3}. Local Endpoint: {4}",
                User != null ? User.Username : "unknown",
                User != null ? User.UserId : -1,
                message,
                TcpClient.Client.RemoteEndPoint,
                TcpClient.Client.LocalEndPoint);
        }


        public void ReceiveMessage()
        {
            Byte[] bytesReceived = new Byte[256];
            string page = String.Empty;
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[ipHostInfo.AddressList.Length - 1];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any,
            IPEndPoint.Parse(TcpClient.Client.LocalEndPoint.ToString()).Port);

            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25999);
            
            Socket _listener = new Socket(ipAddress.AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(localEndPoint);
            _listener.Connect(remoteEndPoint);
            _listener.Listen(100);

            while (true)
            {
                int bytes = 0;

                Socket handler = _listener.Accept();
                bytes = handler.Receive(bytesReceived);
                /*
                // The following will block until the page is transmitted.
                do
                {
                    bytes = stream.Read(bytesReceived, 0, bytesReceived.Length);
                    page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);
                IMessage message = MessageSerializer.Deserialize(Encoding.ASCII.GetBytes(page));
                */
                IMessage message = MessageSerializer.Deserialize(bytesReceived);
                if (message.MessageTypeId == PFire.Core.Protocol.Messages.XFireMessageType.LoginChallenge)
                {
                    LoginRequest request = new LoginRequest();
                    this.SendMessage(request);
                }
                if (message.MessageTypeId == PFire.Core.Protocol.Messages.XFireMessageType.LoginFailure)
                {
                    LoginFailure failure = (LoginFailure)message;
                    Console.WriteLine($"Login failed!: {failure.Reason}");

                }
                if (message.MessageTypeId == PFire.Core.Protocol.Messages.XFireMessageType.LoginSuccess)
                {
                    Console.WriteLine("Login succeeded!");
                }
            }
        }

        public void TestSend(byte[] data)
        {
            Debug.WriteLine("[XFireClient] Sent message to client {0}: {1}", TcpClient.Client.RemoteEndPoint, BitConverter.ToString(data));
            TcpClient.Client.Send(data);
        }
    }
}
