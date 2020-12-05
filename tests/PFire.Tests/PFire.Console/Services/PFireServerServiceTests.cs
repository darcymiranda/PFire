using System.Threading.Tasks;
using Moq;
using PFire.Console.Services;
using PFire.Core;
using Xunit;

namespace PFire.Tests.PFire.Console.Services
{
    public class PFireServerServiceTests : BaseTest
    {
        [Fact]
        public async Task StartAsync_Calls_Start()
        {
            //arrange
            var pFireServerMock = _autoMoqer.GetMock<IPFireServer>();

            var service = _autoMoqer.CreateInstance<PFireServerService>();

            //act
            await service.StartAsync(default);

            //assert
            pFireServerMock.Verify(x => x.Start(), Times.Once);
            pFireServerMock.Verify(x => x.Stop(), Times.Never);
        }

        [Fact]
        public async Task StopAsync_Calls_Stop()
        {
            //arrange
            var pFireServerMock = _autoMoqer.GetMock<IPFireServer>();

            var service = _autoMoqer.CreateInstance<PFireServerService>();

            //act
            await service.StopAsync(default);

            //assert
            pFireServerMock.Verify(x => x.Start(), Times.Never);
            pFireServerMock.Verify(x => x.Stop(), Times.Once);
        }
    }
}
