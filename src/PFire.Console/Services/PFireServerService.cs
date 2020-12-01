using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PFire.Core;

namespace PFire.WindowsService.Services
{
    internal class PFireServerService : IHostedService
    {
        private readonly PFireServer _pfServer;

        public PFireServerService(PFireServer pFireServer)
        {
            _pfServer = pFireServer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _pfServer.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _pfServer.Stop();

            return Task.CompletedTask;
        }
    }
}