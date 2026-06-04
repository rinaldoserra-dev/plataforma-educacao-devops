using PlataformaEducacao.GestaoAluno.Application.DTO;
using PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels;

namespace PlataformaEducacao.GestaoAluno.Application.Queries
{
    public interface IAlunoQueries
    {
        Task<MatriculaViewModel?> ObterMatricula(Guid matriculaId, CancellationToken cancellationToken);
        Task<IEnumerable<MatriculaPendentePagamentoDTO>> ListarMatriculasPendentesPagamentoPorAlunoId(Guid alunoId, CancellationToken cancellationToken);
        Task<IEnumerable<MatriculaAtivaDTO>> ObterMatriculasAtivasPorAlunoId(Guid alunoId, CancellationToken cancellationToken);
        Task<IEnumerable<MatriculaViewModel>> ObterAlunosMatriculadosPorCursoId(Guid cursoId, CancellationToken cancellationToken);
        Task<IEnumerable<MatriculaViewModel>> ObterAlunosPendentesPorCursoId(Guid cursoId, CancellationToken cancellationToken);
        Task<CertificadoDTO?> ValidarCertificado(string codigoVerificacao, CancellationToken cancellationToken);
        Task<ArquivoDTO?> BaixarCertificado(Guid certificadoId, CancellationToken cancellationToken);
        Task<HistoricoAlunoViewModel?> ObterHistoricoAluno(Guid alunoId, CancellationToken cancellationToken);
    }
}