using PFire.Core.Session;
using PFire.Infrastructure.Database;
using System;

namespace PFire.Core.Protocol.Interfaces
{
    public interface IXFireClientManager
    {
        XFireClient GetSession(Guid sessionId);
        XFireClient GetSession(User user);
        void AddSession(XFireClient session);
        void RemoveSession(XFireClient session);
    }
}
