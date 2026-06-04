using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.Request.GestaoConteudo;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.Core.Communication;
using System.Net;

namespace PlataformaEducacao.Bff.Api.Tests.Services
{
    public class CursosServiceTest
    {
        private readonly MockHttpMessageHandler _handler;
        private readonly CursosService _service;

        public CursosServiceTest()
        {
            _handler = new MockHttpMessageHandler();
            var httpClient = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost") };
            var settings = Options.Create(new AppServicesSettings { GestaoConteudoUrl = "http://localhost" });
            _service = new CursosService(httpClient, settings);
        }

        [Fact(DisplayName = "AdicionarCurso deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - CursosService")]
        public async Task AdicionarCurso_DeveRetornarResponseResult()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });
            var request = new AdicionarCursoRequest { Nome = "Curso", DescricaoConteudo = "Desc", CargaHoraria = 10, Valor = 100, Disponivel = true };

            var result = await _service.AdicionarCurso(request);

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "AtualizarCurso deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - CursosService")]
        public async Task AtualizarCurso_DeveRetornarResponseResult()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });
            var id = Guid.NewGuid();
            var request = new AtualizarCursoRequest { Id = id, Nome = "Curso Alt", DescricaoConteudo = "Desc", CargaHoraria = 10, Valor = 200, Disponivel = true };

            var result = await _service.AtualizarCurso(id, request);

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "AdicionarAula deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - CursosService")]
        public async Task AdicionarAula_DeveRetornarResponseResult()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });
            var request = new AdicionarAulaRequest { CursoId = Guid.NewGuid(), Titulo = "Aula", Conteudo = "Conteudo", Ordem = 1 };

            var result = await _service.AdicionarAula(request);

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "ObterCursosDisponiveisComAula deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - CursosService")]
        public async Task ObterCursosDisponiveisComAula_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var result = await _service.ObterCursosDisponiveisComAula();

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "ObterCursoComAulasPorCursoId deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - CursosService")]
        public async Task ObterCursoComAulasPorCursoId_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var result = await _service.ObterCursoComAulasPorCursoId(Guid.NewGuid());

            Assert.True(result.Sucesso);
        }

        [Fact(DisplayName = "ObterTodos deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - CursosService")]
        public async Task ObterTodos_DeveRetornar()
        {
            _handler.SetupResponse(HttpStatusCode.OK, new ResponseResult { Sucesso = true, Status = 200, Erros = new ResponseErrorMessages() });

            var result = await _service.ObterTodos();

            Assert.True(result.Sucesso);
        }
    }
}
