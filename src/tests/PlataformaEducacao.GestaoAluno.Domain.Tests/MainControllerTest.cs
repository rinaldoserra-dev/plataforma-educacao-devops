using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Core.Communication;
using PlataformaEducacao.WebApi.Core.Controllers;
using System.Net;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests
{
    public class MainControllerTest
    {
        [Fact(DisplayName = "CustomResponse sem erros deve retornar OK com sucesso")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void CustomResponse_SemErros_DeveRetornarOkComSucesso()
        {
            var controller = new TestableMainController();

            var result = controller.TestCustomResponse();

            var objResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ResponseResult>(objResult.Value);
            Assert.True(response.Sucesso);
        }

        [Fact(DisplayName = "CustomResponse com erros deve retornar BadRequest")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void CustomResponse_ComErros_DeveRetornarBadRequest()
        {
            var controller = new TestableMainController();
            controller.TestAdicionarErro("Erro genérico");

            var result = controller.TestCustomResponse();

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseResult>(badRequest.Value);
            Assert.False(response.Sucesso);
            Assert.Contains("Erro genérico", response.Erros.Mensagens);
        }

        [Fact(DisplayName = "CustomResponse com dados deve retornar dados no response")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void CustomResponse_ComDados_DeveRetornarDadosNoResponse()
        {
            var controller = new TestableMainController();

            var result = controller.TestCustomResponse(HttpStatusCode.OK, "dados de teste");

            var objResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ResponseResult>(objResult.Value);
            Assert.Equal("dados de teste", response.Data);
        }

        [Fact(DisplayName = "CustomResponse com ValidationResult válido deve retornar OK")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void CustomResponse_ValidationResultValido_DeveRetornarOk()
        {
            var controller = new TestableMainController();
            var validationResult = new ValidationResult();

            var result = controller.TestCustomResponseValidation(validationResult);

            var objResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ResponseResult>(objResult.Value);
            Assert.True(response.Sucesso);
        }

        [Fact(DisplayName = "CustomResponse com ValidationResult inválido deve retornar BadRequest")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void CustomResponse_ValidationResultInvalido_DeveRetornarBadRequest()
        {
            var controller = new TestableMainController();
            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("Campo", "Erro no campo"));

            var result = controller.TestCustomResponseValidation(validationResult);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseResult>(badRequest.Value);
            Assert.False(response.Sucesso);
            Assert.Contains("Erro no campo", response.Erros.Mensagens);
        }

        [Fact(DisplayName = "CustomResponse com ModelState inválido deve retornar BadRequest")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void CustomResponse_ModelStateInvalido_DeveRetornarBadRequest()
        {
            var controller = new TestableMainController();
            controller.ModelState.AddModelError("Nome", "Nome obrigatório");

            var result = controller.TestCustomResponseModelState();

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseResult>(badRequest.Value);
            Assert.Contains("Nome obrigatório", response.Erros.Mensagens);
        }

        [Fact(DisplayName = "CustomResponse com ResponseResult sucesso deve retornar OK")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void CustomResponse_ResponseResultSucesso_DeveRetornarOk()
        {
            var controller = new TestableMainController();
            var responseResult = new ResponseResult
            {
                Sucesso = true,
                Erros = new ResponseErrorMessages()
            };

            var result = controller.TestCustomResponseResult(responseResult);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact(DisplayName = "CustomResponse com ResponseResult null deve retornar BadRequest")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void CustomResponse_ResponseResultNull_DeveRetornarBadRequest()
        {
            var controller = new TestableMainController();

            var result = controller.TestCustomResponseResult(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact(DisplayName = "ResponseContemErros com erros deve adicionar erros e retornar true")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void ResponseContemErros_ComErros_DeveRetornarTrue()
        {
            var controller = new TestableMainController();
            var response = new ResponseResult
            {
                Erros = new ResponseErrorMessages { Mensagens = ["Erro 1", "Erro 2"] }
            };

            var result = controller.TestResponseContemErros(response);

            Assert.True(result);
        }

        [Fact(DisplayName = "ResponseContemErros sem erros deve retornar false")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void ResponseContemErros_SemErros_DeveRetornarFalse()
        {
            var controller = new TestableMainController();
            var response = new ResponseResult
            {
                Erros = new ResponseErrorMessages()
            };

            var result = controller.TestResponseContemErros(response);

            Assert.False(result);
        }

        [Fact(DisplayName = "ResponseContemErros com null deve retornar false")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void ResponseContemErros_ComNull_DeveRetornarFalse()
        {
            var controller = new TestableMainController();

            var result = controller.TestResponseContemErros(null!);

            Assert.False(result);
        }

        [Fact(DisplayName = "LimparErrosProcessamento deve limpar todos os erros")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void LimparErrosProcessamento_DeveLimparErros()
        {
            var controller = new TestableMainController();
            controller.TestAdicionarErro("Erro");
            controller.TestLimparErros();

            var result = controller.TestCustomResponse();

            var objResult = Assert.IsType<ObjectResult>(result);
            var response = Assert.IsType<ResponseResult>(objResult.Value);
            Assert.True(response.Sucesso);
        }

        [Fact(DisplayName = "OperacaoValida com erros deve retornar false")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void OperacaoValida_ComErros_DeveRetornarFalse()
        {
            var controller = new TestableMainController();
            controller.TestAdicionarErro("Erro");

            Assert.False(controller.TestOperacaoValida());
        }

        [Fact(DisplayName = "OperacaoValida sem erros deve retornar true")]
        [Trait("Categoria", "WebApi.Core - Controllers - MainController")]
        public void OperacaoValida_SemErros_DeveRetornarTrue()
        {
            var controller = new TestableMainController();

            Assert.True(controller.TestOperacaoValida());
        }

        private class TestableMainController : MainController
        {
            public ActionResult TestCustomResponse(HttpStatusCode statusCode = HttpStatusCode.OK, object? data = null)
                => CustomResponse(statusCode, data);

            public ActionResult TestCustomResponseValidation(ValidationResult validationResult)
                => CustomResponse(validationResult);

            public ActionResult TestCustomResponseModelState()
                => CustomResponse(ModelState);

            public ActionResult TestCustomResponseResult(ResponseResult response)
                => CustomResponse(response);

            public bool TestResponseContemErros(ResponseResult response)
                => ResponseContemErros(response);

            public void TestAdicionarErro(string msg) => AdicionarErroProcessamento(msg);

            public void TestLimparErros() => LimparErrosProcessamento();

            public bool TestOperacaoValida() => OperacaoValida();
        }
    }
}
