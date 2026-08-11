using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using PlataformaEducacao.Bff.Api;

namespace PlataformaEducacao.Bff.Api.Tests.Config
{

    public class ResponseErrorMessages
    {
        public List<string> Mensagens { get; set; } = new();
    }
}
