﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PFire.Protocol;
using PFire.Protocol.Messages;
using PFire.Protocol.XFireAttributes;
using PFire.Session;

namespace PFire
{
    public class TcpServer
    {
        public delegate void OnReceiveHandler(XFireClient sessionContext, IMessage message);
        public event OnReceiveHandler OnReceive;

        public delegate void OnConnectionHandler(XFireClient sessionContext);
        public event OnConnectionHandler OnConnection;

        public delegate void OnDisconnectionHandler(XFireClient sessionContext);
        public event OnDisconnectionHandler OnDisconnection;

        private readonly TcpListener _listener;
        private bool _running;
        private PFireServer _server;

        public TcpServer(IPEndPoint endPoint, PFireServer server)
        {
            _listener = new TcpListener(endPoint);
            _server = server;
        }

        public TcpServer(IPAddress ip, int port, PFireServer server) : this(new IPEndPoint(ip, port), server) { }

        public void Listen()
        {
            _running = true;
            _listener.Start();
            Task.Run(() => Accept().ConfigureAwait(false));
        }

        public void Shutdown()
        {
            _listener.Stop();
            _running = false;
        }

        private async Task Accept()
        {
            while (_running)
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                XFireClient session = new XFireClient(tcpClient);
                Debug.WriteLine("Client connected {0} and assigned session id {1}", session.TcpClient.Client.RemoteEndPoint, session.SessionId);

                OnConnection?.Invoke(session);

#pragma warning disable 4014
                // Fire and forget. Can't be bothered to fix right now. This whole class needs to be rewritten and decoupled
                Receive(session);
#pragma warning restore 4014
            }
        }

        private async Task Receive(XFireClient context)
        {
            var stream = context.TcpClient.GetStream();
            while (_running)
            {
                // First time the client connects, an opening statement of 4 bytes is sent that needs to be ignored
                if (!context.Initialized)
                {
                    var openingStatementBuffer = new byte[4];
                    await stream.ReadAsync(openingStatementBuffer, 0, openingStatementBuffer.Length);
                    context.InitializeClient(_server);
                }

                // Header determines size of message
                var headerBuffer = new byte[2];
                var read = await stream.ReadAsync(headerBuffer, 0, headerBuffer.Length);
                if (read == 0)
                {
                    OnDisconnection?.Invoke(context);
                    break;
                }

                var messageLength = BitConverter.ToInt16(headerBuffer, 0) - headerBuffer.Length;
                var messageBuffer = new byte[messageLength];
                read = await stream.ReadAsync(messageBuffer, 0, messageLength);

                Debug.WriteLine("RECEIVED RAW: " + BitConverter.ToString(messageBuffer));

                try
                {
                    IMessage message = MessageSerializer.Deserialize(messageBuffer);
                    Console.WriteLine("Recv message[{0},{1}]: {2}",
                        context.User != null ? context.User.Username : "unknown",
                        context.User != null ? context.User.UserId : -1,
                        message);
                    OnReceive?.Invoke(context, message);
                }
                catch (UnknownMessageTypeException messageTypeEx)
                {
                    Debug.WriteLine(messageTypeEx.ToString());
                }
                catch (UnknownXFireAttributeTypeException attributeTypeEx)
                {
                    Debug.WriteLine(attributeTypeEx.ToString());
                }
            }
        }
    }
}
