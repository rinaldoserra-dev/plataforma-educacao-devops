using Microsoft.Extensions.Options;
using Moq;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.GestaoAlunos;
using PlataformaEducacao.Bff.Api.Models.GestaoConteudo;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.Core.Communication;
using System.Net;
using System.Text.Json;

namespace PlataformaEducacao.Bff.Api.Tests.Services
{
    public class AlunosServiceTest
    {
        private readonly MockHttpMessageHandler _handler;
        private readonly Mock<ICursosService> _cursosServiceMock;
        private readonly AlunosService _service;

        public AlunosServiceTest()
        {
            _handler = new MockHttpMessageHandler();
            var httpClient = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost") };
            _cursosServiceMock = new Mock<ICursosService>();
            var settings = Options.Create(new AppServicesSettings { GestaoAlunosUrl = "http://localhost" });
            _service = new AlunosService(httpClient, _cursosServiceMock.Object, settings);
        }

        [Fact(DisplayName = "Matricular com dados do curso preenchidos deve enviar direto")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task Matricular_ComDadosPreenchidos_DeveEnviarDireto()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });
            var dto = new MatricularDTO { CursoId = Guid.NewGuid(), NomeCurso = "Curso", TotalAulasCurso = 5, Valor = 100m };

            var result = await _service.Matricular(dto);

            Assert.True(result.Sucesso);
            _cursosServiceMock.Verify(c => c.ObterCursoComAulasPorCursoId(It.IsAny<Guid>()), Times.Never);
        }

        [Fact(DisplayName = "Matricular sem dados do curso deve buscar no CursosService")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task Matricular_SemDadosCurso_DeveBuscarNoCursosService()
        {
            var cursoId = Guid.NewGuid();
            var curso = new CursoDetalhesDTO
            {
                Id = cursoId,
                Nome = "Curso C#",
                Valor = 500m,
                Disponivel = true,
                Aulas = [new AulaResumoDTO { Id = Guid.NewGuid() }]
            };
            var cursoJson = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(curso));
            _cursosServiceMock.Setup(c => c.ObterCursoComAulasPorCursoId(cursoId))
                .ReturnsAsync(new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages(), Data = cursoJson });

            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var dto = new MatricularDTO { CursoId = cursoId };

            var result = await _service.Matricular(dto);

            Assert.True(result.Sucesso);
            _cursosServiceMock.Verify(c => c.ObterCursoComAulasPorCursoId(cursoId), Times.Once);
        }

        [Fact(DisplayName = "Matricular quando CursosService retorna erro deve retornar erro")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task Matricular_CursoServiceComErro_DeveRetornarErro()
        {
            var cursoId = Guid.NewGuid();
            _cursosServiceMock.Setup(c => c.ObterCursoComAulasPorCursoId(cursoId))
                .ReturnsAsync(new ResponseResult { Sucesso = false, Status = 400, Erros = new ResponseErrorMessages { Mensagens = ["Curso não encontrado"] } });

            var dto = new MatricularDTO { CursoId = cursoId };

            var result = await _service.Matricular(dto);

            Assert.False(result.Sucesso);
        }

        [Fact(DisplayName = "Matricular quando curso é null (data inválida) deve retornar 502")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task Matricular_CursoNull_DeveRetornar502()
        {
            var cursoId = Guid.NewGuid();
            _cursosServiceMock.Setup(c => c.ObterCursoComAulasPorCursoId(cursoId))
                .ReturnsAsync(new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages(), Data = null });

            var dto = new MatricularDTO { CursoId = cursoId };

            var result = await _service.Matricular(dto);

            Assert.False(result.Sucesso);
            Assert.Equal(502, result.Status);
        }

        [Fact(DisplayName = "Matricular quando curso não disponível deve retornar 400")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task Matricular_CursoIndisponivel_DeveRetornar400()
        {
            var cursoId = Guid.NewGuid();
            var curso = new CursoDetalhesDTO
            {
                Id = cursoId,
                Nome = "Curso",
                Valor = 500m,
                Disponivel = false,
                Aulas = [new AulaResumoDTO { Id = Guid.NewGuid() }]
            };
            var cursoJson = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(curso));
            _cursosServiceMock.Setup(c => c.ObterCursoComAulasPorCursoId(cursoId))
                .ReturnsAsync(new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages(), Data = cursoJson });

            var dto = new MatricularDTO { CursoId = cursoId };

            var result = await _service.Matricular(dto);

            Assert.False(result.Sucesso);
            Assert.Equal(400, result.Status);
            Assert.Contains("O curso informado nao esta disponivel para matricula.", result.Erros.Mensagens);
        }

        [Fact(DisplayName = "Matricular quando curso sem aulas deve retornar 400")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task Matricular_CursoSemAulas_DeveRetornar400()
        {
            var cursoId = Guid.NewGuid();
            var curso = new CursoDetalhesDTO
            {
                Id = cursoId,
                Nome = "Curso",
                Valor = 500m,
                Disponivel = true,
                Aulas = Enumerable.Empty<AulaResumoDTO>()
            };
            var cursoJson = JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(curso));
            _cursosServiceMock.Setup(c => c.ObterCursoComAulasPorCursoId(cursoId))
                .ReturnsAsync(new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages(), Data = cursoJson });

            var dto = new MatricularDTO { CursoId = cursoId };

            var result = await _service.Matricular(dto);

            Assert.False(result.Sucesso);
            Assert.Equal(400, result.Status);
            Assert.Contains("O curso informado ainda nao possui aulas cadastradas.", result.Erros.Mensagens);
        }

        [Fact(DisplayName = "ObterMatriculasPendentesPagamento deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task ObterMatriculasPendentes_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var result = await _service.ObterMatriculasPendentesPagamento();

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "ObterMatriculasAtivas deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task ObterMatriculasAtivas_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var result = await _service.ObterMatriculasAtivas();

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "ValidarCertificado deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task ValidarCertificado_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var result = await _service.ValidarCertificado("CERT-123");

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "RealizarAula deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task RealizarAula_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });
            var dto = new RealizarAulaDTO { MatriculaId = Guid.NewGuid(), AulaId = Guid.NewGuid() };

            var result = await _service.RealizarAula(dto);

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "FinalizarCurso deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task FinalizarCurso_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });
            var dto = new FinalizarCursoDTO { MatriculaId = Guid.NewGuid() };

            var result = await _service.FinalizarCurso(dto);

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "ObterHistorico deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task ObterHistorico_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var result = await _service.ObterHistorico(Guid.NewGuid());

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "BaixarCertificado deve retornar HttpResponseMessage")]
        [Trait("Categoria", "Bff.Api - Services - AlunosService")]
        public async Task BaixarCertificado_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK);

            var result = await _service.BaixarCertificado(Guid.NewGuid());

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
