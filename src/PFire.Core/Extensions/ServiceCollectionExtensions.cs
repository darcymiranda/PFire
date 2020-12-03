using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using PFire.Core.Session;

namespace PFire.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCore(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton<IPFireServer, PFireServer>()
                                    .AddSingleton<IXFireClientManager, XFireClientManager>()
                                    .AddSingleton<ITcpServer, TcpServer>()
                                    .AddSingleton(x => new TcpListener(new IPEndPoint(IPAddress.Any, 25999)));
        }
    }
}
