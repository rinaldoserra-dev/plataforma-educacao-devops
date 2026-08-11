using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.GestaoConteudo.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlataformaEducacao.GestaoConteudo.Api.Tests.Config
{

    public class ResponseResult
    {
        public bool Sucesso { get; set; }
        public int Status { get; set; }
        public object? Data { get; set; }
        public ResponseErrorMessages? Erros { get; set; }
    }
}
