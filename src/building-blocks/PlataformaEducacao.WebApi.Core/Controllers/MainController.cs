using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PlataformaEducacao.Core.Communication;
using System.Net;

namespace PlataformaEducacao.WebApi.Core.Controllers
{
    [ApiController]
    public abstract class MainController : Controller
    {
        protected ICollection<string> Errors = [];

        protected ActionResult CustomResponse(HttpStatusCode statusCode = HttpStatusCode.OK, object? data = null)
        {
            if (OperacaoValida())
            {
                return new ObjectResult(new ResponseResult
                {
                    Sucesso = true,
                    Status = (int)statusCode,
                    Erros = new ResponseErrorMessages(),
                    Data = data
                })
                {
                    StatusCode = (int)statusCode
                };
            }

            return BadRequest(new ResponseResult
            {
                Sucesso = false,
                Status = (int)HttpStatusCode.BadRequest,
                Data = null,
                Erros = new ResponseErrorMessages
                {
                    Mensagens = [.. Errors]
                }
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros) AdicionarErroProcessamento(erro.ErrorMessage);

            return CustomResponse();
        }

        protected ActionResult CustomResponse(ValidationResult validationResult)
        {
            foreach (var erro in validationResult.Errors)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
            }

            return CustomResponse();
        }

        protected ActionResult CustomResponse(ResponseResult response)
        {
            if (response is null || response.Sucesso is false || response.Erros.Mensagens.Count > 0)
                return BadRequest(response);

            return Ok(response);
        }

        protected bool ResponseContemErros(ResponseResult responseResult)
        {
            if (responseResult is null || responseResult.Erros.Mensagens.Count == 0)
                return false;

            foreach (var mensagemErro in responseResult.Erros.Mensagens)
                AdicionarErroProcessamento(mensagemErro);

            return true;
        }

        protected void AdicionarErroProcessamento(string mensagem)
        {
            Errors.Add(mensagem);
        }

        protected bool OperacaoValida()
        {
            return Errors.Count == 0;
        }

        protected void LimparErrosProcessamento()
        {
            Errors.Clear();
        }
    }
}