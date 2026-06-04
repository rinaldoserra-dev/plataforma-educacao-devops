using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlataformaEducacao.Bff.Api.Tests.Config
{
    [CollectionDefinition(nameof(IntegrationApiTestsFixtureCollection))]
    public class IntegrationApiTestsFixtureCollection : ICollectionFixture<IntegrationTestsFixture<PlataformaEducacao.Bff.Api.BffApiAssemblyMarker>> { }

    public class IntegrationTestsFixture<TProgram> : IDisposable where TProgram : class
    {
        public readonly PlataformaEducacaoBffAppFactory<TProgram> Factory;
        public HttpClient Client { get; }

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            };

            Factory = new PlataformaEducacaoBffAppFactory<TProgram>();
            Client = Factory.CreateClient(clientOptions);
        }

        public async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                }) ?? throw new InvalidOperationException("Deserialization returned null");
        }

        public IEnumerable<string> GetErrors(string jsonResponse)
        {
            var response = JsonSerializer.Deserialize<ResponseResult>(
                jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return response?.Erros?.Mensagens ?? Enumerable.Empty<string>();
        }

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }

    public class ResponseApi<T>
    {
        public bool Sucesso { get; set; }
        public int Status { get; set; }
        public T? Data { get; set; }
        public ResponseErrorMessages Erros { get; set; } = new();
    }

    public class ResponseResult
    {
        public bool Sucesso { get; set; }
        public int Status { get; set; }
        public object? Data { get; set; }
        public ResponseErrorMessages Erros { get; set; } = new();
    }

    public class ResponseErrorMessages
    {
        public List<string> Mensagens { get; set; } = new();
    }
}
