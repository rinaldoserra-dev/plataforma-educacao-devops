using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.Request.GestaoConteudo;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Services
{
    public interface ICursosService
    {
        Task<ResponseResult> AdicionarCurso(AdicionarCursoRequest cursoRequest);
        Task<ResponseResult> AtualizarCurso(Guid cursoId, AtualizarCursoRequest cursoRequest);
        Task<ResponseResult> AdicionarAula(AdicionarAulaRequest aulaRequest);
        Task<ResponseResult> ObterCursosDisponiveisComAula();
        Task<ResponseResult> ObterCursoComAulasPorCursoId(Guid cursoId);
        Task<ResponseResult> ObterTodos();


    }
    public class CursosService : Service, ICursosService
    {
        private readonly HttpClient _httpClient;

        public CursosService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.GestaoConteudoUrl);
        }

        public async Task<ResponseResult> AdicionarCurso(AdicionarCursoRequest cursoRequest)
        {
            var cursoContent = ObterConteudo(cursoRequest);

            var response = await _httpClient.PostAsync("/api/cursos/", cursoContent);

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> AtualizarCurso(Guid cursoId, AtualizarCursoRequest cursoRequest)
        {
            var cursoContent = ObterConteudo(cursoRequest);

            var response = await _httpClient.PutAsync($"/api/cursos/{cursoId}", cursoContent);

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> AdicionarAula(AdicionarAulaRequest aulaRequest)
        {
            var aulaContent = ObterConteudo(aulaRequest);

            var response = await _httpClient.PostAsync("/api/cursos/aula", aulaContent);

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> ObterCursosDisponiveisComAula()
        {
            var response = await _httpClient.GetAsync("/api/cursos/listar-cursos-disponiveis");

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> ObterCursoComAulasPorCursoId(Guid cursoId)
        {
            var response = await _httpClient.GetAsync($"/api/cursos/{cursoId}");

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> ObterTodos()
        {
            var response = await _httpClient.GetAsync("/api/cursos/listar-todos-cursos");

            return await DeserializarObjetoResponse(response);
        }
    }
}
