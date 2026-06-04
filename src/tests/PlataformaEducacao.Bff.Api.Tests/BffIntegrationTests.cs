using FluentAssertions;
using PlataformaEducacao.Bff.Api.Models.GestaoAlunos;
using PlataformaEducacao.Bff.Api.Models.GestaoFinanceira;
using PlataformaEducacao.Bff.Api.Tests.Config;
using System.Net;
using System.Net.Http.Json;

namespace PlataformaEducacao.Bff.Api.Tests
{
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class BffIntegrationTests : IClassFixture<IntegrationTestsFixture<PlataformaEducacao.Bff.Api.BffApiAssemblyMarker>>
    {
        private readonly IntegrationTestsFixture<PlataformaEducacao.Bff.Api.BffApiAssemblyMarker> _fixture;
        private readonly HttpClient _client;

        public BffIntegrationTests(IntegrationTestsFixture<PlataformaEducacao.Bff.Api.BffApiAssemblyMarker> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
            _client.DefaultRequestHeaders.Clear();
        }

        [Fact(DisplayName = nameof(Health_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF")]
        public async Task Health_DeveRetornarComSucesso()
        {
            var response = await _client.GetAsync("/health");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(Autenticar_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF")]
        public async Task Autenticar_DeveRetornarComSucesso()
        {
            var response = await _client.PostAsJsonAsync("/identidade/autenticar", new
            {
                Email = "aluno@teste.com",
                Senha = "Teste@123"
            });

            response.EnsureSuccessStatusCode();
            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(PagarMatricula_ComoAluno_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF")]
        public async Task PagarMatricula_ComoAluno_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ALUNO");

            var request = new PagarMatriculaDTO
            {
                MatriculaId = Guid.NewGuid(),
                AlunoId = Guid.NewGuid(),
                Valor = 199.90m,
                NomeCartao = "Felicio",
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = "123"
            };

            var response = await _client.PostAsJsonAsync("/pagamentos/matriculas/pagar", request);
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(AdicionarCurso_ComoAluno_DeveRetornarForbidden))]
        [Trait("Categoria", "Integracao API - BFF")]
        public async Task AdicionarCurso_ComoAluno_DeveRetornarForbidden()
        {
            _client.AutenticarComo("ALUNO");

            var response = await _client.PostAsJsonAsync("/cursos", new
            {
                Nome = "Novo Curso",
                DescricaoConteudo = "Descricao",
                CargaHoraria = 10,
                Valor = 100,
                Disponivel = true
            });

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact(DisplayName = nameof(PagarMatricula_SemAutenticacao_DeveRetornarUnauthorized))]
        [Trait("Categoria", "Integracao API - BFF")]
        public async Task PagarMatricula_SemAutenticacao_DeveRetornarUnauthorized()
        {
            _client.AtribuirJsonMediaType();

            var response = await _client.PostAsJsonAsync("/pagamentos/matriculas/pagar", new
            {
                MatriculaId = Guid.NewGuid(),
                Valor = 199.90m,
                NomeCartao = "Felicio",
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = "123"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
