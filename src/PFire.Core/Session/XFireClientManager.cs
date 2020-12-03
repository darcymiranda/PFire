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
        private readonly ConcurrentDictionary<Guid, IXFireClient> _sessions;

        public XFireClientManager()
        {
            _sessions = new ConcurrentDictionary<Guid, IXFireClient>();
        }

        public void AddSession(IXFireClient session)
        {
            if (!_sessions.TryAdd(session.SessionId, session))
            {
                Console.WriteLine("Tried to add a user with session id {0} that already existed", session.SessionId);
            }
        }

        public IXFireClient GetSession(Guid sessionId)
        {
            return _sessions.TryGetValue(sessionId, out var result) ? result : null;
        }

        public IXFireClient GetSession(User user)
        {
            var keyValuePair = _sessions.ToList().FirstOrDefault(a => a.Value.User == user);

            if (!keyValuePair.Equals(default(KeyValuePair<Guid, IXFireClient>)))
            {
                return keyValuePair.Value;
            }

            return null;
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
