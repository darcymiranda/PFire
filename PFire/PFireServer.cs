using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol;
using PFire.Protocol.Messages;
using PFire.Extensions;
using System.Diagnostics;
using PFire.Protocol.XFireAttributes;
using PFire.Session;
using SQLite;
using PFire.Database;
using PFire.Protocol.Messages.Outbound;

namespace PFire
{
    public class PFireServer
    {
        public PFireDatabase Database { get; private set; }

        private TcpServer server;
        private Dictionary<Guid, Context> sessions;

        public PFireServer()
        {
            Database = new PFireDatabase();
            sessions = new Dictionary<Guid, Context>();
            server = new TcpServer(IPAddress.Any, 25999);
            server.OnReceive += HandleRequest;
            server.OnConnection += HandleNewConnection;
            server.OnDisconnection += OnDisconnection;
            server.StartListening();
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
                    sessionContext.SendAndProcessMessage(new FriendsStatus(friend));
                }
            });
        }

        void HandleNewConnection(Context sessionContext)
        {
            AddSession(sessionContext);
        }

        void HandleRequest(Context context, IMessage message)
        {
            context.Server = this;
            //Console.WriteLine("Processing: {0}", message.ToString());
            message.Process(context);
        }

        public Context GetSession(Guid sessionId)
        {
            return sessions[sessionId];
        }

        public Context GetSession(User user)
        {
            var keyValuePair = sessions.ToList().FirstOrDefault(a => a.Value.User == user);
            if (!keyValuePair.Equals(default(KeyValuePair<Guid, Context>)))
            {
                return keyValuePair.Value;
            }
            return null;
        }

        public int AddSession(Context session)
        {
            if (sessions.ContainsKey(session.SessionId))
            {
                Debug.WriteLine("Tried to add a user with session id {0} that already existed", "WARN", session.SessionId);
                return -1;
            }
            sessions.Add(session.SessionId, session);
            return sessions.Count;
        }

        public void RemoveSession(Context session)
        {
            RemoveSession(session.SessionId);
        }

        public void RemoveSession(Guid sessionId)
        {
            sessions.Remove(sessionId);
        }

        public void RemoveSession(User user)
        {
            var keyValuePair = sessions.ToList().FirstOrDefault(a => a.Value.User == user);
            if (!keyValuePair.Equals(default(KeyValuePair<Guid, Context>)))
            {
                sessions.Remove(keyValuePair.Key);
            }
        }
    }
}
