using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
}
