using PlataformaEducacao.Core.Data;

namespace PlataformaEducacao.GestaoAluno.Domain.Repositories
{
    public interface IAlunoRepository : IRepository<Aluno>
    {
        Task Inserir(Aluno aluno, CancellationToken cancellationToken);
        Task AtualizarMatricula(Matricula matricula, CancellationToken cancellationToken);
        Task AtualizarProgressoAula(ProgressoAula progressoAula, CancellationToken cancellationToken);
        Task<Aluno?> ObterComMatriculasPorId(Guid alunoId, CancellationToken cancellationToken);
        Task RealizarMatricula(Matricula matricula, CancellationToken cancellationToken);
        Task<Matricula?> ObterMatriculaComAlunoPorId(Guid matriculaId, CancellationToken cancellationToken);
        Task<Matricula?> ObterMatriculaComProgressoAulasPorId(Guid matriculaId, CancellationToken cancellationToken);
        Task<Matricula?> ObterMatriculaComCertificadoPorId(Guid matriculaId, CancellationToken cancellationToken);
        Task<IEnumerable<Matricula>> ListarMatriculasPendentesPagamentoPorAlunoId(Guid alunoId, CancellationToken cancellationToken);
        Task<IEnumerable<Matricula>> ObterAlunosMatriculadosPorCursoId(Guid cursoId, CancellationToken cancellationToken);
        Task<IEnumerable<Matricula>> ObterAlunosPendentesPorCursoId(Guid cursoId, CancellationToken cancellationToken);
        Task<IEnumerable<Matricula>> ObterMatriculasAtivasPorAlunoId(Guid alunoId, CancellationToken cancellationToken);
        Task GerarCertificado(Certificado certificado, CancellationToken cancellationToken);
        Task<Matricula?> ObterCertificadoPorCodigoVerificacao(string codigoVerificacao, CancellationToken cancellationToken);
        Task<Certificado?> ObterCertificadoPorCertificadoId(Guid certificadoId, CancellationToken cancellationToken);
    }
}
