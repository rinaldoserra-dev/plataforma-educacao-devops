using FluentAssertions;
using PlataformaEducacao.Bff.Api.Models.GestaoAlunos;
using PlataformaEducacao.Bff.Api.Tests.Config;
using System.Net;
using System.Net.Http.Json;

namespace PlataformaEducacao.Bff.Api.Tests
{
    [Collection(nameof(IntegrationApiTestsFixtureCollection))]
    public class GestaoAlunosIntegrationTests : IClassFixture<IntegrationTestsFixture<BffApiAssemblyMarker>>
    {
        private readonly IntegrationTestsFixture<BffApiAssemblyMarker> _fixture;
        private readonly HttpClient _client;

        public GestaoAlunosIntegrationTests(IntegrationTestsFixture<BffApiAssemblyMarker> fixture)
        {
            _fixture = fixture;
            _client = fixture.Client;
            _client.DefaultRequestHeaders.Clear();
        }

        [Fact(DisplayName = nameof(ObterMatriculasAtivas_ComoAluno_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task ObterMatriculasAtivas_ComoAluno_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ALUNO");

            var response = await _client.GetAsync("/alunos/matriculas-ativas");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(ObterMatriculasPendentesPagamento_ComoAluno_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task ObterMatriculasPendentesPagamento_ComoAluno_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ALUNO");

            var response = await _client.GetAsync("/alunos/matriculas-pendentes-pagamento");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(ValidarCertificado_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task ValidarCertificado_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ALUNO");

            var response = await _client.GetAsync("/alunos/validar-certificado/ABC123");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(RealizarAula_ComoAluno_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task RealizarAula_ComoAluno_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ALUNO");

            var request = new RealizarAulaDTO
            {
                MatriculaId = Guid.NewGuid(),
                AulaId = Guid.NewGuid()
            };

            var response = await _client.PostAsJsonAsync("/alunos/realizar-aula", request);
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(FinalizarCurso_ComoAluno_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task FinalizarCurso_ComoAluno_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ALUNO");

            var request = new FinalizarCursoDTO
            {
                MatriculaId = Guid.NewGuid()
            };

            var response = await _client.PostAsJsonAsync("/alunos/finalizar-curso", request);
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(ObterHistorico_ComoAluno_DeveRetornarComSucesso))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task ObterHistorico_ComoAluno_DeveRetornarComSucesso()
        {
            _client.AutenticarComo("ALUNO");

            var alunoId = Guid.NewGuid();
            var response = await _client.GetAsync($"/alunos/historico-aluno/{alunoId}");
            response.EnsureSuccessStatusCode();

            var retorno = await _fixture.DeserializeResponse<ResponseApi<object>>(response);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            retorno.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = nameof(BaixarCertificado_ComoAluno_DeveRetornarArquivo))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task BaixarCertificado_ComoAluno_DeveRetornarArquivo()
        {
            _client.AutenticarComo("ALUNO");

            var certificadoId = Guid.NewGuid();
            var response = await _client.GetAsync($"/alunos/baixar-certificado/{certificadoId}");
            response.EnsureSuccessStatusCode();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType!.MediaType.Should().Be("application/pdf");
        }

        [Fact(DisplayName = nameof(Matricular_SemAutenticacao_DeveRetornarUnauthorized))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task Matricular_SemAutenticacao_DeveRetornarUnauthorized()
        {
            _client.AtribuirJsonMediaType();

            var request = new MatricularDTO { CursoId = Guid.NewGuid() };
            var response = await _client.PostAsJsonAsync("/alunos/matricular", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = nameof(RealizarAula_SemAutenticacao_DeveRetornarUnauthorized))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task RealizarAula_SemAutenticacao_DeveRetornarUnauthorized()
        {
            _client.AtribuirJsonMediaType();

            var request = new RealizarAulaDTO { MatriculaId = Guid.NewGuid(), AulaId = Guid.NewGuid() };
            var response = await _client.PostAsJsonAsync("/alunos/realizar-aula", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact(DisplayName = nameof(FinalizarCurso_SemAutenticacao_DeveRetornarUnauthorized))]
        [Trait("Categoria", "Integracao API - BFF - GestaoAlunos")]
        public async Task FinalizarCurso_SemAutenticacao_DeveRetornarUnauthorized()
        {
            _client.AtribuirJsonMediaType();

            var request = new FinalizarCursoDTO { MatriculaId = Guid.NewGuid() };
            var response = await _client.PostAsJsonAsync("/alunos/finalizar-curso", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
