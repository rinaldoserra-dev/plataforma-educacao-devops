using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Bff.Api.Controllers;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Tests.Controllers
{
    // Pequena implementação concreta para expor os métodos protegidos do BaseController
    public class TestBaseController : BaseController
    {
        public ActionResult InvokeCustomResponse(ModelStateDictionary modelState)
        {
            return CustomResponse(modelState);
        }

        public ActionResult InvokeCustomResponse(ResponseResult response)
        {
            return CustomResponse(response);
        }
    }

    public class BaseControllerTest
    {
        [Fact(DisplayName = "CustomResponse com ModelState com erros deve retornar BadRequest com ResponseResult")]
        [Trait("Categoria", "Bff.Api - Controllers - BaseController")]
        public void CustomResponse_ModelStateWithErrors_Returns_BadRequest_With_ResponseResult()
        {
            // Arrange
            var controller = new TestBaseController();
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("campo", "erro de validação");

            // Act
            var result = controller.InvokeCustomResponse(modelState);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var bad = result.As<BadRequestObjectResult>();
            bad.Value.Should().BeOfType<ResponseResult>();
            var resp = bad.Value.As<ResponseResult>();
            resp.Sucesso.Should().BeFalse();
            resp.Status.Should().Be(StatusCodes.Status400BadRequest);
            resp.Erros.Mensagens.Should().Contain("erro de validação");
        }

        [Fact(DisplayName = "CustomResponse com ResponseResult nulo deve retornar BadRequest")]
        [Trait("Categoria", "Bff.Api - Controllers - BaseController")]
        public void CustomResponse_NullResponse_Returns_BadRequest()
        {
            // Arrange
            var controller = new TestBaseController();

            // Act
            ResponseResult? nullResponse = null;
            var result = controller.InvokeCustomResponse(nullResponse);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact(DisplayName = "CustomResponse com ResponseResult com erros ou falha deve retornar BadRequest")]
        [Trait("Categoria", "Bff.Api - Controllers - BaseController")]
        public void CustomResponse_ResponseWithErrorsOrFailure_Returns_BadRequest()
        {
            // Arrange
            var controller = new TestBaseController();
            var response = new ResponseResult
            {
                Sucesso = false,
                Status = StatusCodes.Status500InternalServerError,
                Erros = new ResponseErrorMessages { Mensagens = new System.Collections.Generic.List<string> { "erro" } }
            };

            // Act
            var result = controller.InvokeCustomResponse(response);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact(DisplayName = "CustomResponse com ResponseResult com sucesso e sem erros deve retornar Ok")]
        [Trait("Categoria", "Bff.Api - Controllers - BaseController")]
        public void CustomResponse_SuccessResponseWithNoErrors_Returns_Ok()
        {
            // Arrange
            var controller = new TestBaseController();
            var response = new ResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Erros = new ResponseErrorMessages() // vazio
            };

            // Act
            var result = controller.InvokeCustomResponse(response);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result.As<OkObjectResult>();
            ok.Value.Should().BeSameAs(response);
        }
    }
}
