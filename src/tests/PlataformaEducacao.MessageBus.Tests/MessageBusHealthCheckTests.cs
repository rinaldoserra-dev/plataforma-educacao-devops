using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using PlataformaEducacao.MessageBus.HealthChecks;

namespace PlataformaEducacao.MessageBus.Tests
{
    public class MessageBusHealthCheckTests
    {
        [Fact]
        public async Task CheckHealthAsync_QuandoConectado_DeveRetornarHealthy()
        {
            var bus = new Mock<IMessageBus>();
            bus.SetupGet(b => b.IsConnected).Returns(true);

            var result = await new MessageBusHealthCheck(bus.Object)
                .CheckHealthAsync(new HealthCheckContext());

            Assert.Equal(HealthStatus.Healthy, result.Status);
        }

        [Fact]
        public async Task CheckHealthAsync_QuandoDesconectado_DeveRetornarUnhealthy()
        {
            var bus = new Mock<IMessageBus>();
            bus.SetupGet(b => b.IsConnected).Returns(false);

            var result = await new MessageBusHealthCheck(bus.Object)
                .CheckHealthAsync(new HealthCheckContext());

            Assert.Equal(HealthStatus.Unhealthy, result.Status);
            Assert.Contains("RabbitMQ", result.Description);
        }
    }
}
