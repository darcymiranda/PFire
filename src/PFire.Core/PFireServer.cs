﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using PFire.Protocol.Messages;
using System.Diagnostics;
using PFire.Session;
using PFire.Database;
using PFire.Protocol.Messages.Outbound;
using System.Reflection;

namespace PFire
{
    public class PFireServer
    {
        public PFireDatabase Database { get; }

        private readonly TcpServer _server;
        private readonly Dictionary<Guid, Context> _sessions;

        public PFireServer(string baseDirectory, IPEndPoint endPoint = null)
        {
            
            Database = new PFireDatabase(baseDirectory);
            _sessions = new Dictionary<Guid, Context>();
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

        void OnDisconnection(Context sessionContext)
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

        private void HandleNewConnection(Context sessionContext)
        {
            AddSession(sessionContext);
        }

        private void HandleRequest(Context context, IMessage message)
        {
            context.Server = this;
            message.Process(context);
        }

        public Context GetSession(Guid sessionId)
        {
            return _sessions[sessionId];
        }

        public Context GetSession(User user)
        {
            var keyValuePair = _sessions.ToList().FirstOrDefault(a => a.Value.User == user);
            if (!keyValuePair.Equals(default(KeyValuePair<Guid, Context>)))
            {
                return keyValuePair.Value;
            }
            return null;
        }

        private void AddSession(Context session)
        {
            if (_sessions.ContainsKey(session.SessionId))
            {
                Debug.WriteLine("Tried to add a user with session id {0} that already existed", "WARN", session.SessionId);
                return;
            }
            _sessions.Add(session.SessionId, session);
        }

        public void RemoveSession(Context session)
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
            if (!keyValuePair.Equals(default(KeyValuePair<Guid, Context>)))
            {
                _sessions.Remove(keyValuePair.Key);
            }
        }
    }
}
