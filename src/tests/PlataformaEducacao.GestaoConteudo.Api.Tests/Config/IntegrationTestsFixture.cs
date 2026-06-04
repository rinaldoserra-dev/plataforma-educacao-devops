using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.GestaoConteudo.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlataformaEducacao.GestaoConteudo.Api.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationApiTestsFixtureCollection))]
    public class IntegrationApiTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture<Program>> { }
    public class IntegrationTestsFixture<TProgram> : IDisposable where TProgram : class
    {
        public readonly PlataformaEducacaoGestaoConteudoAppFactory<TProgram> Factory;
        public HttpClient Client;
        private readonly IServiceScope _serviceScope;
        private readonly GestaoConteudoContext _gestaoConteudoContext;

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            };

            Factory = new PlataformaEducacaoGestaoConteudoAppFactory<TProgram>();
            Client = Factory.CreateClient(clientOptions);

            _serviceScope = Factory.Services.CreateScope();
            _gestaoConteudoContext = _serviceScope.ServiceProvider.GetRequiredService<GestaoConteudoContext>();
        }


        public GestaoConteudoContext GestaoConteudoContext => _gestaoConteudoContext;

        public async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content!,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                }) ?? throw new InvalidOperationException("Deserialization returned null"); ;
        }

        public IEnumerable<string> GetErrors(string jsonResponse)
        {
            var response = JsonSerializer.Deserialize<ResponseResult>(
                jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return response?.Erros?.Mensagens ?? Enumerable.Empty<string>();
        }
        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
            _serviceScope.Dispose();
        }
    }
    public class ResponseApi<T>
    {
        public bool Sucesso { get; set; }
        public T? Data { get; set; }
    }

    public class ResponseResult
    {
        public bool Sucesso { get; set; }
        public int Status { get; set; }
        public object? Data { get; set; }
        public ResponseErrorMessages? Erros { get; set; }
    }

    public class ResponseErrorMessages
    {
        public List<string> Mensagens { get; set; } = new();
    }
}
