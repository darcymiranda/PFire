using PFire.Database;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
