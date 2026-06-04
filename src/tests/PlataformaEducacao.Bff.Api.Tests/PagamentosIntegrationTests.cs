using FluentAssertions;
using PlataformaEducacao.Bff.Api.Tests.Config;
using System.Net;
using System.Net.Http.Json;

namespace PlataformaEducacao.Bff.Api.Tests
{
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class PagamentosIntegrationTests : IClassFixture<IntegrationTestsFixture<BffApiAssemblyMarker>>
    {
        private readonly IntegrationTestsFixture<BffApiAssemblyMarker> _fixture;
        private readonly HttpClient _client;

        public PagamentosIntegrationTests(IntegrationTestsFixture<BffApiAssemblyMarker> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
            _client.DefaultRequestHeaders.Clear();
        }

        [Fact(DisplayName = nameof(ObterStatusPagamento_ComoAluno_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - Pagamentos")]
        public async Task ObterStatusPagamento_ComoAluno_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ALUNO");

            var matriculaId = Guid.NewGuid();
            var response = await _client.GetAsync($"/pagamentos/matriculas/{matriculaId}/status");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(ObterStatusPagamento_SemAutenticacao_DeveRetornarUnauthorized))]
        [Trait("Categoria", "Integracao API - BFF - Pagamentos")]
        public async Task ObterStatusPagamento_SemAutenticacao_DeveRetornarUnauthorized()
        {
            _client.AtribuirJsonMediaType();

            var matriculaId = Guid.NewGuid();
            var response = await _client.GetAsync($"/pagamentos/matriculas/{matriculaId}/status");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = nameof(PagarMatricula_SemDadosCartao_DeveRetornarBadRequest))]
        [Trait("Categoria", "Integracao API - BFF - Pagamentos")]
        public async Task PagarMatricula_SemDadosCartao_DeveRetornarBadRequest()
        {
            _client.AutenticarComo("ALUNO");

            var response = await _client.PostAsJsonAsync("/pagamentos/matriculas/pagar", new
            {
                MatriculaId = Guid.NewGuid(),
                Valor = 0,
                NomeCartao = "",
                NumeroCartao = "",
                ExpiracaoCartao = "",
                CvvCartao = ""
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact(DisplayName = nameof(Health_QuandoSaudeOk_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - Health")]
        public async Task Health_QuandoSaudeOk_DeveRetornarComSucesso()
        {
            var response = await _client.GetAsync("/health");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }
    }
}
