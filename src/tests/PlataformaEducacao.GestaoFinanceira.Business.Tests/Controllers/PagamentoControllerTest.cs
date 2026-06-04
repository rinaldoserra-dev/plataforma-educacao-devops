using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlataformaEducacao.Core.Communication;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoFinanceira.Api.Controllers;
using PlataformaEducacao.GestaoFinanceira.Api.Models.Requests;
using PlataformaEducacao.GestaoFinanceira.Api.Models.Response;
using PlataformaEducacao.GestaoFinanceira.Api.Services;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.WebApi.Core.Usuario;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.Controllers
{
    public class PagamentoControllerTest
    {
        private readonly Mock<IPagamentoService> _pagamentoServiceMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IAspNetUser> _userMock;
        private readonly PagamentoController _controller;

        public PagamentoControllerTest()
        {
            _pagamentoServiceMock = new Mock<IPagamentoService>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _userMock = new Mock<IAspNetUser>();
            _userMock.Setup(u => u.ObterUserId()).Returns(Guid.NewGuid());

            _controller = new PagamentoController(
                _pagamentoServiceMock.Object,
                _serviceProviderMock.Object,
                _userMock.Object);
        }

        [Fact(DisplayName = "PagarMatricula com sucesso deve retornar OK")]
        [Trait("Categoria", "Gestão Financeira - Controllers - PagamentoController")]
        public async Task PagarMatricula_ComSucesso_DeveRetornarOk()
        {
            // Arrange
            var alunoId = Guid.NewGuid();

            var request = new PagarMatriculaRequest
            {
                MatriculaId = Guid.NewGuid(),
                AlunoId = alunoId,
                Valor = 100m,
                NomeCartao = "Fulano",
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = "123"
            };

            _userMock.Setup(x => x.PossuiRole("ADMIN")).Returns(false);
            _userMock.Setup(x => x.ObterUserId()).Returns(alunoId);

            var response = new ResponseMessage(new ValidationResult());
            _pagamentoServiceMock
                .Setup(s => s.AutorizarPagamento(It.IsAny<Pagamento>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.PagarMatricula(request, CancellationToken.None);

            // Assert
            var objResult = Assert.IsType<ObjectResult>(result);
            var resposta = Assert.IsType<ResponseResult>(objResult.Value);
            Assert.True(resposta.Sucesso);
        }

        [Fact(DisplayName = "PagarMatricula com falha deve retornar BadRequest")]
        [Trait("Categoria", "Gestão Financeira - Controllers - PagamentoController")]
        public async Task PagarMatricula_ComFalha_DeveRetornarBadRequest()
        {
            // Arrange
            var request = new PagarMatriculaRequest
            {
                MatriculaId = Guid.NewGuid(),
                Valor = 100m,
                NomeCartao = "Fulano",
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = "123"
            };

            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("Pagamento", "Pagamento recusado"));
            var response = new ResponseMessage(validationResult);

            _pagamentoServiceMock
                .Setup(s => s.AutorizarPagamento(It.IsAny<Pagamento>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.PagarMatricula(request, CancellationToken.None);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var resposta = Assert.IsType<ResponseResult>(badRequest.Value);
            Assert.False(resposta.Sucesso);
        }

        [Theory(DisplayName = "ObterStatus com pagamento existente deve retornar OK")]
        [Trait("Categoria", "Gestão Financeira - Controllers - PagamentoController")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ObterStatus_PagamentoExistente_DeveRetornarOk(bool isAdmin)
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();

            var statusResponse = new PagamentoStatusResponse
            {
                MatriculaId = matriculaId,
                Status = "Pago"
            };

            _userMock.Setup(x => x.PossuiRole("ADMIN")).Returns(isAdmin);
            _userMock.Setup(x => x.ObterUserId()).Returns(alunoId);

            _pagamentoServiceMock
                .Setup(s => s.ObterStatusPorMatricula(matriculaId, alunoId, isAdmin))
                .ReturnsAsync(statusResponse);

            // Act
            var result = await _controller.ObterStatus(matriculaId, CancellationToken.None);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var responseResult = Assert.IsType<ResponseResult>(objectResult.Value);

            Assert.True(responseResult.Sucesso);
            Assert.Equal(200, responseResult.Status);

            var pagamentoStatus = Assert.IsType<PagamentoStatusResponse>(responseResult.Data);
            Assert.Equal("Pago", pagamentoStatus.Status);
        }

        [Theory(DisplayName = "ObterStatus sem pagamento deve retornar PagamentoPendente")]
        [Trait("Categoria", "Gestão Financeira - Controllers - PagamentoController")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ObterStatus_SemPagamento_DeveRetornarPagamentoPendente(bool isAdmin)
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            _pagamentoServiceMock
                .Setup(s => s.ObterStatusPorMatricula(matriculaId, It.IsAny<Guid>(), isAdmin))
                .ReturnsAsync((PagamentoStatusResponse?)null);

            // Act
            var result = await _controller.ObterStatus(matriculaId, CancellationToken.None);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var responseResult = Assert.IsType<ResponseResult>(objectResult.Value);

            Assert.True(responseResult.Sucesso);
            Assert.Equal(200, responseResult.Status);

            var pagamentoStatus = Assert.IsType<PagamentoStatusResponse>(responseResult.Data);
            Assert.Equal("Pagamento Pendente", pagamentoStatus.Status);
        }
    }
}
