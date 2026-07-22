using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            return BadRequest(new ResponseResult
            {
                Sucesso = false,
                Status = StatusCodes.Status400BadRequest,
                Erros = new ResponseErrorMessages
                {
                    Mensagens = modelState.Values.SelectMany(value => value.Errors)
                                                 .Select(error => error.ErrorMessage)
                                                 .ToList()
                }
            });
        }

        protected ActionResult CustomResponse(ResponseResult response)
        {
            if (response == null || !response.Sucesso || response.Erros.Mensagens.Any())
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
