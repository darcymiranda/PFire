using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PFire.Core.Protocol.Interfaces;
using PFire.Core.Protocol.Messages;
using PFire.Core.Session;
using PFire.Core.Util;

namespace PFire.Core
{
    internal sealed class TcpServer
    {
        public delegate void OnConnectionHandler(XFireClient sessionContext);

        public delegate void OnDisconnectionHandler(XFireClient sessionContext);

        public delegate void OnReceiveHandler(XFireClient sessionContext, IMessage message);

        private readonly IXFireClientManager _clientManager;
        private readonly TcpListener _listener;
        private bool _running;

        public TcpServer(IPEndPoint endPoint, IXFireClientManager clientManager)
        {
            _listener = new TcpListener(endPoint);
            _clientManager = clientManager;
        }

        public event OnConnectionHandler OnConnection;
        public event OnDisconnectionHandler OnDisconnection;
        public event OnReceiveHandler OnReceive;

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
