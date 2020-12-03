using System;
using System.Threading.Tasks;
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
        private readonly ITcpServer _server;

        public PFireServer(IPFireDatabase pFireDatabase, IXFireClientManager xFireClientManager, ITcpServer server)
        {
            Database = pFireDatabase;
            _clientManager = xFireClientManager;

            _server = server;
            _server.OnReceive += HandleRequest;
            _server.OnConnection += HandleNewConnection;
            _server.OnDisconnection += OnDisconnection;
        }

        public IPFireDatabase Database { get; }

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

        private void OnDisconnection(IXFireClient disconnectedClient)
        {
            // we have to remove the session first 
            // because of the friends of this user processing
            RemoveSession(disconnectedClient);

            UpdateFriendsWithDisconnectedStatus(disconnectedClient);
        }

        private void UpdateFriendsWithDisconnectedStatus(IXFireClient disconnectedClient)
        {
            var friends = Database.QueryFriends(disconnectedClient.User);

            friends.ForEach(friend =>
            {
                var friendClient = GetSession(friend);
                if (friendClient != null)
                {
                    friendClient.SendAndProcessMessage(new FriendsSessionAssign(friend));
                }
            });
        }

        private void HandleNewConnection(IXFireClient sessionContext)
        {
            AddSession(sessionContext);
        }

        private void HandleRequest(IXFireClient context, IMessage message)
        {
            context.Server = this;
            message.Process(context);
        }

        public IXFireClient GetSession(Guid sessionId)
        {
            return _clientManager.GetSession(sessionId);
        }

        public IXFireClient GetSession(User user)
        {
            return _clientManager.GetSession(user);
        }

        private void AddSession(IXFireClient session)
        {
            _clientManager.AddSession(session);
        }

        public void RemoveSession(IXFireClient session)
        {
            _clientManager.RemoveSession(session);
        }
    }
}
