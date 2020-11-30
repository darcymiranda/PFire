using PFire.Core.Protocol.Interfaces;
using PFire.Infrastructure.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PFire.Core.Session
{
    internal sealed class XFileClientManager : IXFireClientManager
    {
        private readonly ConcurrentDictionary<Guid, XFireClient> _sessions;
        public XFileClientManager()
        {
            _sessions = new ConcurrentDictionary<Guid, XFireClient>();
        }

        public void AddSession(XFireClient session)
        {
            if (!_sessions.TryAdd(session.SessionId, session))
            {
                Console.WriteLine("Tried to add a user with session id {0} that already existed", "WARN", session.SessionId);
            }
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

        public void RemoveSession(XFireClient session)
        {
            RemoveSession(session.SessionId);
        }

        public void RemoveSession(Guid sessionId)
        {
            XFireClient currentSesson;
            if (_sessions.TryRemove(sessionId, out currentSesson))
            {
                currentSesson.DisconnectAndStop();
                currentSesson.Dispose();
            }
        }
    }
}
