using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using PlataformaEducacao.Bff.Api;

namespace PlataformaEducacao.Bff.Api.Tests.Config
{

    public class ResponseResult
    {
        public bool Sucesso { get; set; }
        public int Status { get; set; }
        public object? Data { get; set; }
        public ResponseErrorMessages Erros { get; set; } = new();
    }
}
