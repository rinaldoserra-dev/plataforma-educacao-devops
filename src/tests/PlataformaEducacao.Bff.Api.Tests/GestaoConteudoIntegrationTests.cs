using FluentAssertions;
using PlataformaEducacao.Bff.Api.Tests.Config;
using System.Net;
using System.Net.Http.Json;

namespace PlataformaEducacao.Bff.Api.Tests
{
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class GestaoConteudoIntegrationTests : IClassFixture<IntegrationTestsFixture<BffApiAssemblyMarker>>
    {
        private readonly IntegrationTestsFixture<BffApiAssemblyMarker> _fixture;
        private readonly HttpClient _client;

        public GestaoConteudoIntegrationTests(IntegrationTestsFixture<BffApiAssemblyMarker> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
            _client.DefaultRequestHeaders.Clear();
        }

        [Fact(DisplayName = nameof(ListarCursosDisponiveis_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoConteudo")]
        public async Task ListarCursosDisponiveis_DeveRetornarComSucesso()
        {
            var response = await _client.GetAsync("/cursos/listar-cursos-disponiveis");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(ObterDetalhesCurso_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoConteudo")]
        public async Task ObterDetalhesCurso_DeveRetornarComSucesso()
        {
            var cursoId = Guid.NewGuid();
            var response = await _client.GetAsync($"/cursos/{cursoId}");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(AdicionarCurso_ComoAdmin_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoConteudo")]
        public async Task AdicionarCurso_ComoAdmin_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ADMIN");

            var response = await _client.PostAsJsonAsync("/cursos", new
            {
                Nome = "Curso de Docker",
                DescricaoConteudo = "Descricao do Curso de Docker",
                CargaHoraria = 15,
                Valor = 400,
                Disponivel = true
            });

            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(AtualizarCurso_ComoAdmin_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoConteudo")]
        public async Task AtualizarCurso_ComoAdmin_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ADMIN");

            var cursoId = Guid.NewGuid();
            var response = await _client.PutAsJsonAsync($"/cursos/{cursoId}", new
            {
                Id = cursoId,
                Nome = "Curso Atualizado",
                DescricaoConteudo = "Descricao Atualizada",
                CargaHoraria = 20,
                Valor = 500,
                Disponivel = true
            });

            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(AdicionarAula_ComoAdmin_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoConteudo")]
        public async Task AdicionarAula_ComoAdmin_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ADMIN");

            var response = await _client.PostAsJsonAsync("/cursos/aula", new
            {
                CursoId = Guid.NewGuid(),
                Titulo = "Aula 1",
                Conteudo = "Conteudo da Aula",
                Ordem = 1,
                Material = "Material da aula"
            });

            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(ListarTodosCursos_ComoAdmin_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoConteudo")]
        public async Task ListarTodosCursos_ComoAdmin_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ADMIN");

            var response = await _client.GetAsync("/cursos/listar-todos-cursos");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(ListarTodosCursos_SemAutenticacao_DeveRetornarUnauthorized))]
        [Trait("Categoria", "Integracao API - BFF - GestaoConteudo")]
        public async Task ListarTodosCursos_SemAutenticacao_DeveRetornarUnauthorized()
        {
            _client.AtribuirJsonMediaType();

            var response = await _client.GetAsync("/cursos/listar-todos-cursos");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = nameof(AdicionarAula_SemAutenticacao_DeveRetornarUnauthorized))]
        [Trait("Categoria", "Integracao API - BFF - GestaoConteudo")]
        public async Task AdicionarAula_SemAutenticacao_DeveRetornarUnauthorized()
        {
            _client.AtribuirJsonMediaType();

            var response = await _client.PostAsJsonAsync("/cursos/aula", new
            {
                CursoId = Guid.NewGuid(),
                Titulo = "Aula 1",
                Conteudo = "Conteudo",
                Ordem = 1
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = nameof(NovoAluno_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - Identidade")]
        public async Task NovoAluno_DeveRetornarComSucesso()
        {
            var response = await _client.PostAsJsonAsync("/identidade/novo-aluno", new
            {
                Nome = "Aluno Teste",
                Email = "aluno@teste.com",
                Senha = "Teste@123",
                SenhaConfirmacao = "Teste@123"
            });

            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            retorno.Sucesso.Should().BeTrue();
        }
    }
}
