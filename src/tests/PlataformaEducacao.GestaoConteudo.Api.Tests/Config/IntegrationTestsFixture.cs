using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.GestaoConteudo.Data;

namespace PlataformaEducacao.GestaoConteudo.Api.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationApiTestsCollectionFixture))]
    public class IntegrationApiTestsCollectionFixture : ICollectionFixture<IntegrationTestsFixture<Program>>
    {
    }
}
