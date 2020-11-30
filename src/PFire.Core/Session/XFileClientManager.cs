using PFire.Core.Protocol.Interfaces;
using PFire.Database;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PFire.Core.Session
{
    internal sealed class XFileClientManager : IXFireClientManager
    {
        // the Dictionary<> is thread safe when read, but not when modified
        private readonly Dictionary<Guid, XFireClient> _sessions;
        private readonly  object _lock;
        public XFileClientManager()
        {
            _sessions = new Dictionary<Guid, XFireClient>();
            _lock = new object();
        }

        public void AddSession(XFireClient session)
        {
            if (_sessions.ContainsKey(session.SessionId))
            {
                Console.WriteLine("Tried to add a user with session id {0} that already existed", "WARN", session.SessionId);
                return;
            }

            lock (_lock)
            {
                _sessions.Add(session.SessionId, session);
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
            lock (_lock)
            {
                _sessions.Remove(sessionId);
            }
        }
    }
}
