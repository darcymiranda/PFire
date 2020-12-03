using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using PFire.Core.Protocol.Interfaces;
using PFire.Core.Protocol.Messages;
using PFire.Core.Session;
using PFire.Core.Util;

namespace PFire.Core
{
    internal interface ITcpServer
    {
        event Action<XFireClient, IMessage> OnReceive;
        event Action<XFireClient> OnConnection;
        event Action<XFireClient> OnDisconnection;
        void Listen();
        void Shutdown();
    }

    internal sealed class TcpServer : ITcpServer
    {
        private readonly IXFireClientManager _clientManager;
        private readonly TcpListener _listener;
        private bool _running;

        public TcpServer(TcpListener listener, IXFireClientManager clientManager)
        {
            _listener = listener;
            _clientManager = clientManager;
        }

        public event Action<XFireClient> OnConnection;
        public event Action<XFireClient> OnDisconnection;
        public event Action<XFireClient, IMessage> OnReceive;

        public void Listen()
        {
            _running = true;
            _listener.Start();
            ConsoleLogger.Log($"PFire Server listening on {_listener.LocalEndpoint}");
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
                var tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                var newXFireClient = new XFireClient(tcpClient, _clientManager, OnReceive, OnDisconnection);

                OnConnection?.Invoke(newXFireClient);
            }
        }
    }
}
