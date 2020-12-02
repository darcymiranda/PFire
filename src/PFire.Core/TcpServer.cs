using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using PFire.Core.Protocol.Interfaces;
using PFire.Core.Protocol.Messages;
using PFire.Core.Session;

namespace PFire.Core
{
    public sealed class TcpServer
    {
        public delegate void OnReceiveHandler(XFireClient sessionContext, IMessage message);
        public event OnReceiveHandler OnReceive;

        public delegate void OnConnectionHandler(XFireClient sessionContext);
        public event OnConnectionHandler OnConnection;

        public delegate void OnDisconnectionHandler(XFireClient sessionContext);
        public event OnDisconnectionHandler OnDisconnection;

        private readonly TcpListener _listener;
        private bool _running;
        private readonly IXFireClientManager _clientManager;

        public TcpServer(IPEndPoint endPoint, IXFireClientManager clientManager)
        {
            _listener = new TcpListener(endPoint);
            _clientManager = clientManager;
        }

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
                var tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                var session = new XFireClient(tcpClient, _clientManager, OnReceive, OnDisconnection);

                OnConnection?.Invoke(session);
            }
        }
    }
}
