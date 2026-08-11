using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.GestaoConteudo.Data;

namespace PlataformaEducacao.GestaoConteudo.Api.Tests.Config
{

    public class IntegrationTestsFixture<TProgram> : IDisposable where TProgram : class
    {
        public readonly PlataformaEducacaoGestaoConteudoAppFactory<TProgram> Factory;
        public HttpClient Client;
        private readonly IServiceScope _serviceScope;

        public IntegrationTestsFixture()
        {
            var clientOptions = new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            };

            Factory = new PlataformaEducacaoGestaoConteudoAppFactory<TProgram>();
            Client = Factory.CreateClient(clientOptions);

            _serviceScope = Factory.Services.CreateScope();
            GestaoConteudoContext = _serviceScope.ServiceProvider.GetRequiredService<GestaoConteudoContext>();
        }


        public GestaoConteudoContext GestaoConteudoContext { get; }

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
}
