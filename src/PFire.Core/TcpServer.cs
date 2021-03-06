﻿using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PFire.Core.Session;

namespace PFire.Core
{
    internal sealed class TcpServer : ITcpServer
    {
        private readonly IXFireClientManager _clientManager;
        private readonly TcpListener _listener;
        private readonly ILogger<TcpServer> _logger;
        private bool _running;

        public TcpServer(TcpListener listener, IXFireClientManager clientManager, ILogger<TcpServer> logger)
        {
            _listener = listener;
            _clientManager = clientManager;
            _logger = logger;
        }

        public event ITcpServer.OnReceiveHandler OnReceive;
        public event ITcpServer.OnConnectionHandler OnConnection;
        public event ITcpServer.OnDisconnectionHandler OnDisconnection;

        public async Task Listen()
        {
            _running = true;
            _listener.Start();
            _logger.LogInformation($"PFire Server listening on {_listener.LocalEndpoint}");
            await Accept().ConfigureAwait(false);
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
                var newXFireClient = new XFireClient(tcpClient, _clientManager, _logger, OnReceive, OnDisconnection);

                OnConnection?.Invoke(newXFireClient);
            }
        }
    }
}
