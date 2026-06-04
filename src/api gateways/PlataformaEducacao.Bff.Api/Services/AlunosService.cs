using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.GestaoAlunos;
using PlataformaEducacao.Bff.Api.Models.GestaoConteudo;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Services
{
    public interface IAlunosService
    {
        Task<ResponseResult> Matricular(MatricularDTO solicitarMatricula);
        Task<ResponseResult> ObterMatriculasPendentesPagamento();
        Task<ResponseResult> ObterMatriculasAtivas();
        Task<ResponseResult> ValidarCertificado(string codigoVerificacao);
        Task<ResponseResult> RealizarAula(RealizarAulaDTO realizarAula);
        Task<ResponseResult> FinalizarCurso(FinalizarCursoDTO finalizarCurso);
        Task<ResponseResult> ObterHistorico(Guid alunoId);
        Task<HttpResponseMessage> BaixarCertificado(Guid certificadoId);
    }

    public class AlunosService : Service, IAlunosService
    {
        private readonly HttpClient _httpClient;
        private readonly ICursosService _cursosService;

        public AlunosService(HttpClient httpClient,
                             ICursosService cursosService,
                             IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _cursosService = cursosService;
            _httpClient.BaseAddress = new Uri(settings.Value.GestaoAlunosUrl);
        }

        public async Task<ResponseResult> Matricular(MatricularDTO matricular)
        {
            if (DadosCursoPreenchidos(matricular) is false)
            {
                var cursoResponse = await _cursosService.ObterCursoComAulasPorCursoId(matricular.CursoId);
                if (cursoResponse.Sucesso is false || cursoResponse.Erros.Mensagens.Any())
                {
                    return cursoResponse;
                }

                var curso = DeserializarData<CursoDetalhesDTO>(cursoResponse);
                if (curso is null)
                {
                    return new ResponseResult
                    {
                        Status = StatusCodes.Status502BadGateway,
                        Sucesso = false,
                        Erros = new ResponseErrorMessages
                        {
                            Mensagens = ["Nao foi possivel carregar os dados do curso para matricula."]
                        }
                    };
                }

                if (curso.Disponivel is false)
                {
                    return new ResponseResult
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Sucesso = false,
                        Erros = new ResponseErrorMessages
                        {
                            Mensagens = ["O curso informado nao esta disponivel para matricula."]
                        }
                    };
                }

                if (curso.Aulas.Any() is false)
                {
                    return new ResponseResult
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Sucesso = false,
                        Erros = new ResponseErrorMessages
                        {
                            Mensagens = ["O curso informado ainda nao possui aulas cadastradas."]
                        }
                    };
                }

                matricular.NomeCurso = curso.Nome;
                matricular.Valor = curso.Valor;
                matricular.TotalAulasCurso = curso.Aulas.Count();
            }

            var conteudoMatricular = ObterConteudo(matricular);
            var response = await _httpClient.PostAsync("/api/alunos/matricular", conteudoMatricular);

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> ObterMatriculasPendentesPagamento()
        {
            var response = await _httpClient.GetAsync("/api/alunos/matriculas-pendentes-pagamento");

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> ObterMatriculasAtivas()
        {
            var response = await _httpClient.GetAsync("/api/alunos/matriculas-ativas");

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> ValidarCertificado(string codigoVerificacao)
        {
            var response = await _httpClient.GetAsync($"/api/alunos/validar-certificado/{codigoVerificacao}");

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> RealizarAula(RealizarAulaDTO realizarAula)
        {
            var conteudoRealizarAula = ObterConteudo(realizarAula);
            var response = await _httpClient.PostAsync("/api/alunos/realizar-aula", conteudoRealizarAula);

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> FinalizarCurso(FinalizarCursoDTO finalizarCurso)
        {
            var conteudoFinalizarCurso = ObterConteudo(finalizarCurso);
            var response = await _httpClient.PostAsync("/api/alunos/finalizar-curso", conteudoFinalizarCurso);

            return await DeserializarObjetoResponse(response);
        }

        public async Task<ResponseResult> ObterHistorico(Guid alunoId)
        {
            var response = await _httpClient.GetAsync($"/api/alunos/historico-aluno/{alunoId}");

            return await DeserializarObjetoResponse(response);
        }

        public async Task<HttpResponseMessage> BaixarCertificado(Guid certificadoId)
        {
            return await _httpClient.GetAsync($"/api/alunos/baixar-certificado/{certificadoId}");
        }

        private static bool DadosCursoPreenchidos(MatricularDTO matricular)
        {
            return string.IsNullOrWhiteSpace(matricular.NomeCurso) is false
                   && matricular.TotalAulasCurso > 0
                   && matricular.Valor > 0;
        }
    }
}
