using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PlataformaEducacao.MessageBus.HealthChecks
{
    public sealed class MessageBusHealthCheck(IMessageBus messageBus) : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(messageBus.IsConnected
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("RabbitMQ connection is unavailable."));
        }
    }
}
