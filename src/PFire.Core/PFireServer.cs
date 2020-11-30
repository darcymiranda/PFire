using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using PFire.Core.Protocol.Messages;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;
using PFire.Infrastructure.Database;

namespace PFire.Core
{
    public class PFireServer
    {
        public PFireDatabase Database { get; }

        private readonly TcpServer _server;
        private readonly Dictionary<Guid, XFireClient> _sessions;

        public PFireServer(string baseDirectory, IPEndPoint endPoint = null)
        {
            
            Database = new PFireDatabase(baseDirectory);
            _sessions = new Dictionary<Guid, XFireClient>();
            _server = new TcpServer(endPoint ?? new IPEndPoint(IPAddress.Any, 25999));
            _server.OnReceive += HandleRequest;
            _server.OnConnection += HandleNewConnection;
            _server.OnDisconnection += OnDisconnection;
        }

        public void Start()
        {
            _server.Listen();
        }

        public void Stop()
        {
            _server.Shutdown();
        }

        void OnDisconnection(XFireClient sessionContext)
        {
            RemoveSession(sessionContext);

            var friends = Database.QueryFriends(sessionContext.User);
            friends.ForEach(friend =>
            {
                var friendSession = GetSession(friend);
                if (friendSession != null)
                {
                    // Not working
                    sessionContext.SendAndProcessMessage(new FriendsSessionAssign(friend));
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
            return _sessions[sessionId];
        }

        public XFireClient GetSession(User user)
        {
            var keyValuePair = _sessions.ToList().FirstOrDefault(a => a.Value.User == user);
            if (!keyValuePair.Equals(default(KeyValuePair<Guid, XFireClient>)))
            {
                return keyValuePair.Value;
            }
            return null;
        }

        private void AddSession(XFireClient session)
        {
            if (_sessions.ContainsKey(session.SessionId))
            {
                Debug.WriteLine("Tried to add a user with session id {0} that already existed", "WARN", session.SessionId);
                return;
            }
            _sessions.Add(session.SessionId, session);
        }

        public void RemoveSession(XFireClient session)
        {
            RemoveSession(session.SessionId);
        }

        private void RemoveSession(Guid sessionId)
        {
            _sessions.Remove(sessionId);
        }

        public void RemoveSession(User user)
        {
            var keyValuePair = _sessions.ToList().FirstOrDefault(a => a.Value.User == user);
            if (!keyValuePair.Equals(default(KeyValuePair<Guid, XFireClient>)))
            {
                _sessions.Remove(keyValuePair.Key);
            }
        }
    }
}
