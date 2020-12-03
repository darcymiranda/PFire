using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PFire.Infrastructure.Database;

namespace PFire.Core.Session
{
    internal interface IXFireClientManager
    {
        IXFireClient GetSession(Guid sessionId);
        IXFireClient GetSession(User user);
        void AddSession(IXFireClient session);
        void RemoveSession(IXFireClient session);
    }

    internal sealed class XFireClientManager : IXFireClientManager
    {
        private readonly ConcurrentDictionary<Guid, XFireClient> _sessions;

        public XFireClientManager()
        {
            _sessions = new ConcurrentDictionary<Guid, XFireClient>();
        }

        public void AddSession(XFireClient session)
        {
            if (!_sessions.TryAdd(session.SessionId, session))
            {
                Console.WriteLine("Tried to add a user with session id {0} that already existed", session.SessionId);
            }
        }

        public XFireClient GetSession(Guid sessionId)
        {
            return _sessions.TryGetValue(sessionId, out var result) ? result : null;
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

        public void RemoveSession(XFireClient session)
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
