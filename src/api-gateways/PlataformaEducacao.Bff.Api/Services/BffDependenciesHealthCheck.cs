using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PlataformaEducacao.Bff.Api.Services
{
    public sealed class BffDependenciesHealthCheck(IHealthCheckService healthCheckService) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var result = await healthCheckService.VerificarSaude();
            return result.Sucesso
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("One or more dependent APIs are unavailable.");
        }
    }
}
