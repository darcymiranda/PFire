using System;
using PFire.Core.Session;
using PFire.Infrastructure.Database;

namespace PFire.Core.Protocol.Interfaces
{
    internal interface IXFireClientManager
    {
        XFireClient GetSession(Guid sessionId);
        XFireClient GetSession(User user);
        void AddSession(XFireClient session);
        void RemoveSession(XFireClient session);
    }
}
