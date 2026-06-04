using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Data.Repository
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly GestaoAlunoContext _context;

        public AlunoRepository(GestaoAlunoContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task Inserir(Aluno aluno, CancellationToken cancellationToken)
        {
            await _context.Alunos.AddAsync(aluno, cancellationToken);
        }
        public Task AtualizarMatricula(Matricula matricula, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Matriculas.Update(matricula));
        }
        public async Task AtualizarProgressoAula(ProgressoAula progressoAula, CancellationToken cancellationToken)
        {
            await _context.ProgressoAulas.AddAsync(progressoAula, cancellationToken);
        }

        public async Task GerarCertificado(Certificado certificado, CancellationToken cancellationToken)
        {
            await _context.Certificados.AddAsync(certificado, cancellationToken);
        }
        public async Task<IEnumerable<Matricula>> ListarMatriculasPendentesPagamentoPorAlunoId(Guid alunoId, CancellationToken cancellationToken)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.HistoricoAprendizado)
                .AsNoTracking()
                .Where(m => m.SituacaoMatricula == SituacaoMatricula.PendentePagamento &&
                            m.AlunoId == alunoId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Matricula>> ObterAlunosMatriculadosPorCursoId(Guid cursoId, CancellationToken cancellationToken)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.HistoricoAprendizado)
                .AsNoTracking()
                .Where(m => m.SituacaoMatricula == SituacaoMatricula.Ativa &&
                            m.CursoId == cursoId)
                .ToListAsync(cancellationToken);
        }
        public async Task<IEnumerable<Matricula>> ObterAlunosPendentesPorCursoId(Guid cursoId, CancellationToken cancellationToken)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.HistoricoAprendizado)
                .AsNoTracking()
                .Where(m => m.SituacaoMatricula != SituacaoMatricula.Ativa &&
                            m.CursoId == cursoId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Aluno?> ObterComMatriculasPorId(Guid alunoId, CancellationToken cancellationToken)
        {
            return await _context.Alunos
                .Include(a => a.Matriculas)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == alunoId, cancellationToken);
        }

        public async Task<Matricula?> ObterMatriculaComAlunoPorId(Guid matriculaId, CancellationToken cancellationToken)
        {
            return await _context.Matriculas
                .Include(m => m.HistoricoAprendizado)
                .Include(m => m.Aluno)
                .FirstOrDefaultAsync(m => m.Id == matriculaId, cancellationToken);
        }

        public async Task<Matricula?> ObterMatriculaComProgressoAulasPorId(Guid matriculaId, CancellationToken cancellationToken)
        {
            return await _context.Matriculas
                .Include(m => m.HistoricoAprendizado)
                .Include(m => m.ProgressoAulas)
                .FirstOrDefaultAsync(m => m.Id == matriculaId, cancellationToken);
        }
        public async Task<Matricula?> ObterMatriculaComCertificadoPorId(Guid matriculaId, CancellationToken cancellationToken)
        {
            return await _context.Matriculas
                .Include(m => m.Certificado)
                .FirstOrDefaultAsync(m => m.Id == matriculaId, cancellationToken);
        }

        public async Task<IEnumerable<Matricula>> ObterMatriculasAtivasPorAlunoId(Guid alunoId, CancellationToken cancellationToken)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Certificado)
                .Include(m => m.HistoricoAprendizado)
                .AsNoTracking()
                .Where(m => m.SituacaoMatricula == SituacaoMatricula.Ativa &&
                            m.AlunoId == alunoId)
                .ToListAsync(cancellationToken);
        }

        public async Task RealizarMatricula(Matricula matricula, CancellationToken cancellationToken)
        {
            await _context.Matriculas.AddAsync(matricula, cancellationToken);
        }
        public async Task<Matricula?> ObterCertificadoPorCodigoVerificacao(string codigoVerificacao, CancellationToken cancellationToken)
        {
            return await _context.Matriculas
                .Include(m => m.HistoricoAprendizado)
                .Include(m => m.Aluno)
                .Include(m => m.Certificado)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Certificado!.CodigoVerificacao == codigoVerificacao, cancellationToken);
        }
        public async Task<Certificado?> ObterCertificadoPorCertificadoId(Guid certificadoId, CancellationToken cancellationToken)
        {
            return await _context.Certificados
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.Aluno)
                .Include(c => c.Matricula)
                    .ThenInclude(m => m.HistoricoAprendizado)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == certificadoId, cancellationToken);
        }
        public void Dispose()
        {

        }
    }
}
