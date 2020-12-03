using System;
using System.Net;
using System.Threading.Tasks;
using PFire.Core.Protocol.Interfaces;
using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using PFire.Infrastructure.Database;

namespace PFire.Core
{
    public interface IPFireServer
    {
        Task Start();
        Task Stop();
    }

    internal sealed class PFireServer : IPFireServer
    {
        private readonly IXFireClientManager _clientManager;

        private readonly TcpServer _server;

        public PFireServer(string baseDirectory, IPEndPoint endPoint = null)
        {
            Database = new PFireDatabase(baseDirectory);

            _clientManager = new XFileClientManager();

            _server = new TcpServer(endPoint ?? new IPEndPoint(IPAddress.Any, 25999), _clientManager);
            _server.OnReceive += HandleRequest;
            _server.OnConnection += HandleNewConnection;
            _server.OnDisconnection += OnDisconnection;
        }

        public PFireDatabase Database { get; }

        public Task Start()
        {
            _server.Listen();

            return Task.CompletedTask;
        }

        public Task Stop()
        {
            _server.Shutdown();

            return Task.CompletedTask;
        }

        private void OnDisconnection(XFireClient disconnectedClient)
        {
            // we have to remove the session first 
            // because of the friends of this user processing
            RemoveSession(disconnectedClient);

            UpdateFriendsWithDisconnetedStatus(disconnectedClient);
        }

        private void UpdateFriendsWithDisconnetedStatus(XFireClient disconnectedClient)
        {
            var friends = Database.QueryFriends(disconnectedClient.User);

            friends.ForEach(friend =>
            {
                var friendClient = GetSession(friend);
                if(friendClient != null)
                {
                    friendClient.SendAndProcessMessage(new FriendsSessionAssign(friend));
                }
            });
        }

        private void HandleNewConnection(XFireClient sessionContext)
        {
            AddSession(sessionContext);
        }

        private void HandleRequest(XFireClient context, IMessage message)
        {
            context.Server = this;
            message.Process(context);
        }

        public XFireClient GetSession(Guid sessionId)
        {
            return _clientManager.GetSession(sessionId);
        }

        public XFireClient GetSession(User user)
        {
            return _clientManager.GetSession(user);
        }

        private void AddSession(XFireClient session)
        {
            _clientManager.AddSession(session);
        }

        public void RemoveSession(XFireClient session)
        {
            _clientManager.RemoveSession(session);
        }
    }
}
