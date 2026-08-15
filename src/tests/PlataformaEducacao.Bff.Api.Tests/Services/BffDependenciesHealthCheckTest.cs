using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Tests.Services
{
    public class BffDependenciesHealthCheckTest
    {
        [Fact]
        public async Task CheckHealthAsync_QuandoDependenciasSaudaveis_DeveRetornarHealthy()
        {
            var service = new Mock<IHealthCheckService>();
            service.Setup(s => s.VerificarSaude()).ReturnsAsync(new ResponseResult { Sucesso = true });

            var result = await new BffDependenciesHealthCheck(service.Object)
                .CheckHealthAsync(new HealthCheckContext());

            result.Status.Should().Be(HealthStatus.Healthy);
        }

        [Fact]
        public async Task CheckHealthAsync_QuandoDependenciaIndisponivel_DeveRetornarUnhealthy()
        {
            var service = new Mock<IHealthCheckService>();
            service.Setup(s => s.VerificarSaude()).ReturnsAsync(new ResponseResult { Sucesso = false });

            var result = await new BffDependenciesHealthCheck(service.Object)
                .CheckHealthAsync(new HealthCheckContext());

            result.Status.Should().Be(HealthStatus.Unhealthy);
            result.Description.Should().Contain("dependent APIs");
        }
    }
}
