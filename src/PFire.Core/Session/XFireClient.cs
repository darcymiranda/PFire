﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Logging;
using PFire.Core.Protocol;
using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.XFireAttributes;
using PFire.Core.Util;
using PFire.Infrastructure.Database;

namespace PFire.Core.Session
{
    internal interface IXFireClient
    {
        User User { get; set; }
        Guid SessionId { get; }
        PFireServer Server { get; set; }
        ILogger Logger { get; }
        int PublicIp { get; }
        string Salt { get; }
        void Disconnect();
        void Dispose();
        void SendAndProcessMessage(XFireMessage message);
        void SendMessage(XFireMessage invite);
        void RemoveDuplicatedSessions(User user);
    }

    internal sealed class XFireClient : Disposable, IXFireClient
    {
        private const int ClientTimeoutInMinutes = 5;

        private readonly IXFireClientManager _clientManager;
        private readonly AutoResetEvent _clientWaitEvent;
        private readonly Action<IXFireClient> _disconnectionHandler;
        private readonly object _lock;
        private readonly Action<IXFireClient, IMessage> _receiveHandler;
        private bool _connected;
        private bool _initialized;
        private DateTime _lastReceivedFrom;
        private TcpClient _tcpClient;

        public XFireClient(TcpClient tcpClient,
                           IXFireClientManager clientManager,
                           ILogger logger,
                           Action<IXFireClient, IMessage> receiveHandler,
                           Action<IXFireClient> disconnectionHandler)
        {
            _receiveHandler = receiveHandler;
            _disconnectionHandler = disconnectionHandler;

            _clientManager = clientManager;
            _lock = new object();

            _tcpClient = tcpClient;
            _tcpClient.ReceiveTimeout = 300; // ms
            _connected = true;

            Logger = logger;

            // TODO: be able to use unique salts
            Salt = "4dc383ea21bf4bca83ea5040cb10da62";
            SessionId = Guid.NewGuid();

            _clientWaitEvent = new AutoResetEvent(false);

            _lastReceivedFrom = DateTime.UtcNow;

            Logger.LogInformation($"Client connected {_tcpClient.Client.RemoteEndPoint} and assigned session id {SessionId}");

            ThreadPool.QueueUserWorkItem(ClientThreadWorker);
        }

        private TimeSpan ClientTimeout => TimeSpan.FromMinutes(ClientTimeoutInMinutes);

        public string Salt { get; }

        public PFireServer Server { get; set; }

        public Guid SessionId { get; }

        public User User { get; set; }

        public ILogger Logger { get; }

        public int PublicIp
        {
            get
            {
                IPAddress address;
                var remoteEndPoint = _tcpClient.Client.RemoteEndPoint;
                if (remoteEndPoint is IPEndPoint ipEndPoint)
                {
                    address = ipEndPoint.Address;
                }
                else
                {
                    var addressStr = remoteEndPoint.ToString();
                    var ip = addressStr.Substring(0, addressStr.IndexOf(":"));
                    address = IPAddress.Parse(ip);
                }

                var addressBytes = address.GetAddressBytes();

                return BitConverter.ToInt32(addressBytes);
            }
        }

        public void Disconnect()
        {
            _connected = false;
        }

        public void SendAndProcessMessage(XFireMessage message)
        {
            message.Process(this);
            SendMessage(message);
        }

        public void SendMessage(XFireMessage message)
        {
            if (Disposed)
            {
                // He's dead, Jim.
                return;
            }

            if (!_initialized)
            {
                return;
            }

            var payload = MessageSerializer.Serialize(message);

            _tcpClient.Client.Send(payload);

            var username = User != null ? User.Username : "unknown";
            var userId = User != null ? User.UserId : -1;

            Logger.LogDebug($"Sent message[{username},{userId}]: {message}");
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
            if (DateTime.UtcNow - _lastReceivedFrom > ClientTimeout)
            {
                Logger.LogError($"Client: {User.Username}-{SessionId} has timed out -> {_lastReceivedFrom}");
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

                            if (stream.DataAvailable)
                            {
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
                        }
                        else
                        {
                            // the client says the other end has gone, 
                            // lets shut down this client 
                            Logger.LogError($"Client: {User.Username}-{SessionId} has disconnected");
                            _clientManager.RemoveSession(this);
                        }
                    }
                    catch (IOException ioe)
                    {
                        Logger.LogError(ioe, "An exception occurred when reading from the tcp stream");
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
                Logger.LogCritical($"Client {User?.Username}-{SessionId} disconnected via 0 read");
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

                Logger.LogDebug($"Recv message[{username},{userId}]: {message}");

                _receiveHandler?.Invoke(this, message);
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

        private void ReadOpeningHeader(NetworkStream stream)
        {
            // First time the client connects, an opening statement of 4 bytes is sent that needs to be ignored
            var openingStatementBuffer = new byte[4];
            var read = stream.Read(openingStatementBuffer, 0, openingStatementBuffer.Length);

            _initialized = read == 4;

            if (!_initialized)
            {
                Logger.LogError($"Failed to read header bytes from {SessionId}");
            }
        }
    }
}
