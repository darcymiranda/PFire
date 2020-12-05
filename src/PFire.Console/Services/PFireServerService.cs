using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PFire.Core;

namespace PFire.Console.Services
{
    internal class PFireServerService : IHostedService
    {
        private readonly IPFireServer _pfServer;

        public PFireServerService(IPFireServer pFireServer)
        {
            _pfServer = pFireServer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _pfServer.Start();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _pfServer.Stop();
        }
    }
}