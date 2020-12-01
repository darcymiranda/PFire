using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using PFire.Core.Protocol;
using PFire.Core.Protocol.Interfaces;
using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.XFireAttributes;
using PFire.Core.Util;
using PFire.Infrastructure.Database;

namespace PFire.Core.Session
{
    public sealed class XFireClient : Disposable
    {
        private const int ClientTimeoutInMinutes = 5;

        private readonly IXFireClientManager _clientManager;
        private readonly AutoResetEvent _clientWaitEvent;
        private readonly TcpServer.OnDisconnectionHandler _disconnectionHandler;
        private readonly object _lock;
        private readonly TcpServer.OnReceiveHandler _receiveHandler;
        private bool _connected;
        private bool _initialized;
        private DateTime _lastReceivedFrom;
        private TcpClient _tcpClient;

        public EndPoint RemoteEndPoint => _tcpClient.Client.RemoteEndPoint;

        public string Salt { get; private set;}

        public PFireServer Server { get; set; }

        public Guid SessionId { get; private set;}

        public User User { get; set; }

        public XFireClient(TcpClient tcpClient,
                           IXFireClientManager clientManager,
                           TcpServer.OnReceiveHandler receiveHandler,
                           TcpServer.OnDisconnectionHandler disconnectionHandler)
        {
            _receiveHandler = receiveHandler;
            _disconnectionHandler = disconnectionHandler;

            _clientManager = clientManager;
            _lock = new object();

            _tcpClient = tcpClient;
            _tcpClient.ReceiveTimeout = 300; // ms
            _connected = true;

            // TODO: be able to use unique salts
            Salt = "4dc383ea21bf4bca83ea5040cb10da62";
            SessionId = Guid.NewGuid();

            _clientWaitEvent = new AutoResetEvent(false);

            _lastReceivedFrom = DateTime.UtcNow;

            ConsoleLogger.Log($"Client connected {_tcpClient.Client.RemoteEndPoint} and assigned session id {SessionId}", ConsoleColor.Green);

            ThreadPool.QueueUserWorkItem(ClientThreadWorker);
        }

        public void Disconnect()
        {
            _connected = false;
        }

        // A login has been successful, and as part of the login processing
        // we should remove any duplicate/old sessions
        public void RemoveDuplicatedSessions(User user)
        {
            var otherSession = _clientManager.GetSession(user);
            if (otherSession != null)
            {
                _clientManager.RemoveSession(otherSession);
            }
        }

        public void SendAndProcessMessage(XFireMessage message)
        {
            message.Process(this);
            SendMessage(message);
        }

        public void SendMessage(XFireMessage message)
        {
            if (_initialized)
            {
                var payload = MessageSerializer.Serialize(message);

                _tcpClient.Client.Send(payload);

                var username = User != null ? User.Username : "unknown";
                var userId = User != null ? User.UserId : -1;

                ConsoleLogger.Log($"Sent message[{username},{userId}]: {message}", ConsoleColor.Gray);
            }
        }

        protected override void DisposeManagedResources()
        {
            _clientWaitEvent.Dispose();

            try
            {
                if (_tcpClient.Connected)
                {
                    _tcpClient.Close();
                }
                else
                {
                    _tcpClient.Dispose();
                }
            }
            finally
            {
                _tcpClient = null;
            }
        }

        private void CheckForLifetimeExpiry()
        {
            if (DateTime.UtcNow - _lastReceivedFrom > new TimeSpan(0, ClientTimeoutInMinutes, 0))
            {
                ConsoleLogger.Log($"Client: {User.Username}-{SessionId} has timed out -> {_lastReceivedFrom}", ConsoleColor.Red);
                _clientManager.RemoveSession(this);
            }
        }

        private void ClientThreadWorker(object sender)
        {
            while (_connected)
            {
                lock (_lock)
                {
                    try
                    {
                        if (_tcpClient.Connected)
                        {
                            var stream = _tcpClient.GetStream();

                            if (!_initialized)
                            {
                                ReadOpeningHeader(stream);
                            }
                            else
                            {
                                ReadMessage(stream);
                            }

                            // as we read something (i.e we're still here) we can update the last read time
                            _lastReceivedFrom = DateTime.UtcNow;
                        }
                        else
                        {
                            // the client says the other end has gone, 
                            // lets shut down this client 
                            ConsoleLogger.Log($"Client: {User.Username}-{SessionId} has disconnected", ConsoleColor.Red);
                            _clientManager.RemoveSession(this);
                        }
                    }
                    catch (IOException)
                    {
                        // the read timed out 
                        // this could indicate that the other end is bad
                        // the lifetime handler will help
                    }
                }

                if (_connected)
                {
                    // if the client hasn't disconnected 
                    // we check lifetime and go around again
                    _clientWaitEvent.WaitOne(100);
                    CheckForLifetimeExpiry();
                }
            }
        }

        private void ReadMessage(NetworkStream stream)
        {
            // Header determines size of message
            var headerBuffer = new byte[2];
            var read = stream.Read(headerBuffer, 0, headerBuffer.Length);
            if (read == 0)
            {
                ConsoleLogger.Log($"Client {User.Username}-{SessionId} disconnected via 0 read", ConsoleColor.DarkRed);
                _disconnectionHandler?.Invoke(this);
                return;
            }

            var messageLength = BitConverter.ToInt16(headerBuffer, 0) - headerBuffer.Length;
            var messageBuffer = new byte[messageLength];
            read = stream.Read(messageBuffer, 0, messageLength);

            Debug.WriteLine("RECEIVED RAW: " + BitConverter.ToString(messageBuffer));

            try
            {
                var message = MessageSerializer.Deserialize(messageBuffer);

                var username = User != null ? User.Username : "unknown";
                var userId = User != null ? User.UserId : -1;

                ConsoleLogger.Log($"Recv message[{username},{userId}]: {message}", ConsoleColor.Gray);

                _receiveHandler?.Invoke(this, message);
            }
            catch(UnknownMessageTypeException messageTypeEx)
            {
                Debug.WriteLine(messageTypeEx.ToString());
            }
            catch(UnknownXFireAttributeTypeException attributeTypeEx)
            {
                Debug.WriteLine(attributeTypeEx.ToString());
            }
        }

        private void ReadOpeningHeader(NetworkStream stream)
        {
            // First time the client connects, an opening statement of 4 bytes is sent that needs to be ignored
            var openingStatementBuffer = new byte[4];
            var read = stream.Read(openingStatementBuffer, 0, openingStatementBuffer.Length);

            _initialized = read == 4;

            if (!_initialized)
            {
                ConsoleLogger.Log($"Failed to read header bytes from {SessionId}", ConsoleColor.Red);
            }
        }
    }
}
