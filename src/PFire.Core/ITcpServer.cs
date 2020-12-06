using System.Threading.Tasks;
using PFire.Core.Protocol.Messages;
using PFire.Core.Session;

namespace PFire.Core
{
    internal interface ITcpServer
    {
        delegate void OnConnectionHandler(IXFireClient sessionContext);

        delegate void OnDisconnectionHandler(IXFireClient sessionContext);

        delegate void OnReceiveHandler(IXFireClient sessionContext, IMessage message);

        event OnReceiveHandler OnReceive;
        event OnConnectionHandler OnConnection;
        event OnDisconnectionHandler OnDisconnection;
        Task Listen();
        void Shutdown();
    }
}
