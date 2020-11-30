using System;
using System.Diagnostics;
using System.Net.Sockets;
using PFire.Core.Protocol;
using PFire.Core.Protocol.Messages;
using PFire.Infrastructure.Database;

namespace PFire.Core.Session
{
    public sealed class XFireClient : Disposable
    {
        private const int ClientTimeoutInMinutes = 5;

        public PFireServer Server { get; set; }

        public User User { get; set; }

        public string Salt { get; private set; }
        public Guid SessionId { get; private set; }
        public object TheadPool { get; }

        private readonly object _lock;

        //public TcpClient TcpClient { get; private set; }
        private TcpClient _tcpClient;

        private bool _connected;
        private readonly AutoResetEvent _threadSutdownEvent;
        private readonly AutoResetEvent _clientWaitEvent;

        private DateTime _lastRecivedFrom;
        private bool _initialized;


        public XFireClient(TcpClient tcpClient)
        {
            _lock = new object();

            _tcpClient = tcpClient;
            _tcpClient.ReceiveTimeout = 300; // ms
            _connected = true;

            // TODO: be able to use unique salts
            Salt = "4dc383ea21bf4bca83ea5040cb10da62";//Guid.NewGuid().ToString().Replace("-", string.Empty);
            SessionId = Guid.NewGuid();


            _threadSutdownEvent = new AutoResetEvent(false);
            _clientWaitEvent = new AutoResetEvent(false);

            _lastRecivedFrom = DateTime.UtcNow;

            ThreadPool.QueueUserWorkItem(ClientThreadWorker);
        }

        public void DisconnectAndStop()
        {
            _connected = false;
            _threadSutdownEvent.WaitOne();
        }

        private void ClientThreadWorker(object sender)
        {
            while (_connected)
            {
                lock (_lock)
                {
                    try
                    {
                        using var stream = _tcpClient.GetStream();

                        if (!_initialized)
                        {
                            ReadOpeningHeader(stream);
                        }
                        else
                        {
                            ReadMessage(stream);
                        }

                        // as we read something (i.e we're still here) we can update the last read time
                        _lastRecivedFrom = DateTime.UtcNow;
                    }
                    catch (IOException)
                    {
                        // the read timed out 
                        // this could indicate that the other end is bad
                        // the lifetime handler will help
                    }
                }

                _clientWaitEvent.WaitOne(100);
            }

            _threadSutdownEvent.Set();
        }

        private void ReadMessage(NetworkStream stream)
        {
            throw new NotImplementedException();
        }

        private void ReadOpeningHeader(NetworkStream stream)
        {
            throw new NotImplementedException();
        }

      
        public void SendAndProcessMessage(XFireMessage message)
        {
            message.Process(this);
            SendMessage(message);
        }

        public void SendMessage(XFireMessage message)
        {
            var payload = MessageSerializer.Serialize(message);

            _tcpClient.Client.Send(payload);

            Console.WriteLine("Sent message[{0},{1}]: {2}",
                User != null ? User.Username : "unknown",
                User != null ? User.UserId : -1,
                message);
        }

        protected override void DisposeManagedResources()
        {
            _threadSutdownEvent.Dispose();
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
    }
}
