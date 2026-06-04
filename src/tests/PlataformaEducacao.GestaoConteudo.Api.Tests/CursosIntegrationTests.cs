using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoConteudo.Api.Requests;
using PlataformaEducacao.GestaoConteudo.Api.Tests.Config;
using PlataformaEducacao.GestaoConteudo.Application.Queries.ViewModels;
using PlataformaEducacao.GestaoConteudo.Data;
using System.Net;
using System.Net.Http.Json;

namespace PlataformaEducacao.GestaoConteudo.Api.Tests
{
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class CursosIntegrationTests : IClassFixture<IntegrationTestsFixture<Program>>
    {
        private readonly IntegrationTestsFixture<Program> _fixture;
        private readonly HttpClient _client;
        private readonly GestaoConteudoContext _conteudoContext;

        public CursosIntegrationTests(IntegrationTestsFixture<Program> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
            _conteudoContext = fixture.GestaoConteudoContext;
            _client.DefaultRequestHeaders.Clear();
        }

        [Fact(DisplayName = nameof(ListarCursosDisponiveis_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task ListarCursosDisponiveis_DeveRetornarComSucesso()
        {
            var cursoDisponivel = await _conteudoContext.Cursos
                                        .FirstOrDefaultAsync(c => c.Disponivel);

            var response = await _client.GetAsync("api/cursos/listar-cursos-disponiveis");
            response.EnsureSuccessStatusCode();

            var cursos = await _fixture.DeserializeResponse<ResponseApi<IEnumerable<CursoViewModel>>>(response);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(cursos!.Data!.Any());

            var cursoModel = cursos!.Data!.FirstOrDefault(c => c.Id == cursoDisponivel!.Id);

            Assert.Equal(cursoDisponivel!.Nome, cursoModel!.Nome);
        }

        [Fact(DisplayName = nameof(AdicionarCurso_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AdicionarCurso_DeveRetornarComSucesso()
        {
            var data = new { Nome = "Curso de Docker", DescricaoConteudo = "Descrição do Curso de Docker", CargaHoraria = 15, Valor = 400, Disponivel = true };

            // Injetando o Header para simular um Aluno
            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ADMIN");

            var response = await _client.PostAsJsonAsync("api/cursos", data);
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<string>>(response);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(retorno.Sucesso);

            var cursoInserido = await _conteudoContext.Cursos
                                        .FirstOrDefaultAsync(c => c.Nome == data.Nome);

            Assert.NotNull(cursoInserido);
            Assert.Equal(data.Nome, cursoInserido.Nome);
        }

        [Fact(DisplayName = nameof(AdicionarCurso_ComoAluno_DeveRetornarForbidden))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AdicionarCurso_ComoAluno_DeveRetornarForbidden()
        {
            var data = new { Nome = "Curso de Kubernetes", DescricaoConteudo = "Descrição do Curso de Kubernetes", CargaHoraria = 20, Valor = 600, Disponivel = true };

            // Injetando o Header para simular um Aluno
            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ALUNO");

            // Act
            var response = await _fixture.Client.PostAsJsonAsync("/api/cursos", data);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact(DisplayName = nameof(AdicionarCurso_ComoAllowAnonymous_DeveRetornarForbidden))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AdicionarCurso_ComoAllowAnonymous_DeveRetornarForbidden()
        {
            var data = new { Nome = "Curso de Kubernetes", DescricaoConteudo = "Descrição do Curso de Kubernetes", CargaHoraria = 20, Valor = 600, Disponivel = true };

            _fixture.Client.DefaultRequestHeaders.Clear();

            // Act
            var response = await _fixture.Client.PostAsJsonAsync("/api/cursos", data);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact(DisplayName = nameof(AdicionarCurso_QuandoRequestInvalido_DeveRetornarComFalha))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AdicionarCurso_QuandoRequestInvalido_DeveRetornarComFalha()
        {
            var data = new { Nome = string.Empty, DescricaoConteudo = "Descrição do Curso de Kubernetes", CargaHoraria = 20, Valor = 600, Disponivel = true };

            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ADMIN");

            var response = await _client.PostAsJsonAsync("api/cursos", data);
            var result = _fixture.GetErrors(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(result.Contains("O campo Nome é obrigatório."), "");
            Assert.True(result.Contains("O campo Nome precisa ter entre 2 e 255 caracteres"), "");
        }

        [Fact(DisplayName = nameof(AdicionarAula_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AdicionarAula_DeveRetornarComSucesso()
        {
            var data = new { Nome = "Curso de Kubernetes", DescricaoConteudo = "Descrição do Curso de Kubernetes", CargaHoraria = 20, Valor = 600, Disponivel = true };

            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ADMIN");

            var response = await _client.PostAsJsonAsync("api/cursos", data);

            var curso = await _conteudoContext.Cursos
                                        .FirstOrDefaultAsync(c => c.Nome == data.Nome);

            var aulaRequest = new AdicionarAulaRequest
            {
                CursoId = curso!.Id,
                Conteudo = "Conteudo da aula",
                Material = "Material da aula",
                Ordem = 1,
                Titulo = "Aula 1"
            };

            var responseAula = await _client.PostAsJsonAsync("api/cursos/aula", aulaRequest);
            response.EnsureSuccessStatusCode();
            var retorno = await _fixture.DeserializeResponse<ResponseApi<string>>(response);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(retorno.Sucesso);

            var aulaInserira = await _conteudoContext.Aulas
                                        .FirstOrDefaultAsync(a => a.Conteudo == aulaRequest.Conteudo);

            Assert.NotNull(aulaInserira);
            Assert.Equal(aulaRequest.Material, aulaInserira.Material);
            Assert.Equal(aulaRequest.CursoId, aulaInserira.CursoId);
        }

        [Fact(DisplayName = nameof(AdicionarAula_QuandoRequestInvalido_DeveRetornarComFalha))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AdicionarAula_QuandoRequestInvalido_DeveRetornarComFalha()
        {
            var aula = new AdicionarAulaRequest
            {
                CursoId = Guid.NewGuid(),
                Conteudo = string.Empty,
                Material = "Material da aula",
                Ordem = 1,
                Titulo = "Aula 1"
            };

            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ADMIN");
            var response = await _client.PostAsJsonAsync("api/cursos/aula", aula);
            var result = _fixture.GetErrors(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(result.Contains("O campo Conteudo é obrigatório."), "");
        }

        [Fact(DisplayName = nameof(ObterDetalhesCurso_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task ObterDetalhesCurso_DeveRetornarComSucesso()
        {
            var curso = await _conteudoContext.Cursos
                                        .FirstOrDefaultAsync(c => c.Disponivel);

            var response = await _client.GetAsync($"api/cursos/{curso!.Id}");
            response.EnsureSuccessStatusCode();
            var retorno = await _fixture.DeserializeResponse<ResponseApi<CursoViewModel>>(response);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(retorno.Sucesso);
            Assert.Equal(curso!.Id, retorno.Data!.Id);
            Assert.Equal(curso!.Nome, retorno.Data!.Nome);
        }

        [Fact(DisplayName = nameof(ListarTodosCursos_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task ListarTodosCursos_DeveRetornarComSucesso()
        {
            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ADMIN");

            var response = await _client.GetAsync($"api/cursos/listar-todos-cursos");
            response.EnsureSuccessStatusCode();
            var retorno = await _fixture.DeserializeResponse<ResponseApi<IEnumerable<CursoViewModel>>>(response);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(retorno.Sucesso);
            Assert.True(retorno.Data!.Count() > 0);
        }

        [Fact(DisplayName = "Atualizar Curso")]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AtualizarCurso_DeveRetornarComSucesso()
        {
            var data = new { Nome = "Curso de Java", DescricaoConteudo = "Descrição do Curso de Java", CargaHoraria = 20, Valor = 600, Disponivel = true };

            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ADMIN");

            var response = await _client.PostAsJsonAsync("api/cursos", data);
            var cursoInserido = await _conteudoContext.Cursos
                                        .FirstOrDefaultAsync(c => c.Nome == data.Nome);

            var dataAtualizar = new AtualizarCursoRequest
            {
                Id = cursoInserido!.Id,
                Nome = cursoInserido.Nome + " Alterado",
                DescricaoConteudo = cursoInserido.ConteudoProgramatico.Descricao,
                CargaHoraria = cursoInserido.ConteudoProgramatico.CargaHoraria,
                Disponivel = cursoInserido.Disponivel,
                Valor = cursoInserido.Valor + 1
            };

            response = await _client.PutAsJsonAsync($"api/cursos/{cursoInserido.Id}", dataAtualizar);
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<string>>(response);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(retorno.Sucesso);
        }

        [Fact(DisplayName = nameof(AtualizarCurso_QuandoRequestInvalido_DeveRetornarComFalha))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AtualizarCurso_QuandoRequestInvalido_DeveRetornarComFalha()
        {
            var data = new AtualizarCursoRequest
            {
                Id = Guid.NewGuid(),
                Nome = string.Empty,
                DescricaoConteudo = "Descrição",
                CargaHoraria = 350,
                Disponivel = true,
                Valor = 500
            };

            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ADMIN");

            var response = await _client.PutAsJsonAsync($"api/cursos/{data.Id}", data);
            var result = _fixture.GetErrors(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "Atualizar Curso deve Falhar quando id request diferente id rota")]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AtualizarCurso_QuandoIdRotaDiferenteIdRequest_DeveRetornarComFalha()
        {
            var cursoId = Guid.NewGuid();
            var cursoInvalidoId = Guid.NewGuid();
            var data = new AtualizarCursoRequest
            {
                Id = cursoId,
                Nome = "Curso ABCD",
                DescricaoConteudo = "Descrição",
                CargaHoraria = 350,
                Disponivel = true,
                Valor = 500
            };

            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ADMIN");

            var response = await _client.PutAsJsonAsync($"api/cursos/{cursoInvalidoId}", data);
            var result = _fixture.GetErrors(await response.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = nameof(AtualizarCurso_QuandoNomeExiste_DeveRetornarComFalha))]
        [Trait("Categoria", "Integração API - Curso")]
        public async Task AtualizarCurso_QuandoNomeExiste_DeveRetornarComFalha()
        {
            var cursoRequest = new { Nome = "Curso de Nodejs", DescricaoConteudo = "Descrição do Curso de Nodejs", CargaHoraria = 20, Valor = 600, Disponivel = true };

            _fixture.Client.DefaultRequestHeaders.Add("X-Test-Role", "ADMIN");

            var response = await _client.PostAsJsonAsync("api/cursos", cursoRequest);
            var cursoInserido = await _conteudoContext.Cursos
                                        .FirstOrDefaultAsync(c => c.Nome == cursoRequest.Nome);

            var responseCursos = await _client.GetAsync($"api/cursos/listar-todos-cursos");
            var retornoCursos = await _fixture.DeserializeResponse<ResponseApi<IEnumerable<CursoViewModel>>>(responseCursos);

            var data = new AtualizarCursoRequest
            {
                Id = retornoCursos.Data!.First().Id,
                Nome = cursoInserido!.Nome,
                DescricaoConteudo = cursoInserido.ConteudoProgramatico.Descricao,
                CargaHoraria = cursoInserido.ConteudoProgramatico.CargaHoraria,
                Disponivel = false,
                Valor = cursoInserido.Valor + 1
            };

            response = await _client.PutAsJsonAsync($"api/cursos/{data.Id}", data);
            var result = _fixture.GetErrors(await response.Content.ReadAsStringAsync());

            var retorno = await _fixture.DeserializeResponse<ResponseApi<string>>(response);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(result.Contains("O nome do curso já existe!"), "");
        }
    }
}
