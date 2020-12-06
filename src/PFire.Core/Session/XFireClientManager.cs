﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace PFire.Core.Session
{
    internal interface IXFireClientManager
    {
        IXFireClient GetSession(Guid sessionId);
        IXFireClient GetSession(int userId);
        void AddSession(IXFireClient session);
        void RemoveSession(IXFireClient session);
    }

    internal sealed class XFireClientManager : IXFireClientManager
    {
        private readonly ConcurrentDictionary<Guid, IXFireClient> _sessions;

        public XFireClientManager()
        {
            _sessions = new ConcurrentDictionary<Guid, IXFireClient>();
        }

        public void AddSession(IXFireClient session)
        {
            if (!_sessions.TryAdd(session.SessionId, session))
            {
                session.Logger.LogWarning($"Tried to add a user with session id {session.SessionId} that already existed.");
            }
        }

        public IXFireClient GetSession(Guid sessionId)
        {
            return _sessions.TryGetValue(sessionId, out var result) ? result : null;
        }

        public IXFireClient GetSession(int userId)
        {
            var session = _sessions.ToList().Select(x => x.Value).FirstOrDefault(a => a.User.Id == userId);

            return session == null ? null : GetSession(session.SessionId);
        }

        public void RemoveSession(IXFireClient session)
        {
            RemoveSession(session.SessionId);
        }

        public void RemoveSession(Guid sessionId)
        {
            if (!_sessions.TryRemove(sessionId, out var currentSession))
            {
                return;
            }

            currentSession.Disconnect();
            currentSession.Dispose();
        }
    }
}
