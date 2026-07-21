using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlataformaEducacao.Bff.Api.Controllers;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Tests.Controllers
{
    public class HealthControllerTests
    {
        [Fact(DisplayName = "Get Quando servico de health retorna sucesso Deve retornar Ok")]
        [Trait("Categoria", "Bff.Api - Controllers - HealthController")]
        public async Task Get_QuandoServicoRetornaSucesso_RetornaOk()
        {
            // Arrange
            var expected = new ResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = new { Gateway = "PlataformaEducacao.Bff.Api" }
            };

            var mockService = new Mock<IHealthCheckService>();
            mockService.Setup(s => s.VerificarSaude()).ReturnsAsync(expected);

            var controller = new HealthController(mockService.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseResult>(okResult.Value);
            Assert.True(response.Sucesso);
            Assert.Equal(StatusCodes.Status200OK, response.Status);
        }

        [Fact(DisplayName = "Get Quando serviço de health indica falha Deve retornar 503")]
        [Trait("Categoria", "Bff.Api - Controllers - HealthController")]
        public async Task Get_QuandoServicoRetornaFalha_Retorna503()
        {
            // Arrange
            var expected = new ResponseResult
            {
                Sucesso = false,
                Status = StatusCodes.Status503ServiceUnavailable,
                Erros = new ResponseErrorMessages { Mensagens = { "dependencia indisponivel" } }
            };

            var mockService = new Mock<IHealthCheckService>();
            mockService.Setup(s => s.VerificarSaude()).ReturnsAsync(expected);

            var controller = new HealthController(mockService.Object);

            // Act
            var result = await controller.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status503ServiceUnavailable, objectResult.StatusCode);
            var response = Assert.IsType<ResponseResult>(objectResult.Value);
            Assert.False(response.Sucesso);
        }
    }
}
