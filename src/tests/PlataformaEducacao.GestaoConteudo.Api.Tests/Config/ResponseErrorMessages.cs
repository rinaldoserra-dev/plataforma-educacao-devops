using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.GestaoConteudo.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlataformaEducacao.GestaoConteudo.Api.Tests.Config
{

    public class ResponseErrorMessages
    {
        public List<string> Mensagens { get; set; } = new();
    }
}
