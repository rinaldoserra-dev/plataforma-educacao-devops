using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.GestaoConteudo.Domain;

namespace PlataformaEducacao.GestaoConteudo.Data.Repository
{
    public class CursoRepository : ICursoRepository
    {
        private readonly GestaoConteudoContext _context;

        public CursoRepository(GestaoConteudoContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;
        public async Task Inserir(Curso curso, CancellationToken cancellationToken)
        {
            await _context.Cursos.AddAsync(curso, cancellationToken);
        }

        public Task Atualizar(Curso curso, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Cursos.Update(curso));
        }
        public async Task<Curso?> ObterPorId(Guid cursoId, CancellationToken cancellationToken)
        {
            return await _context.Cursos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cursoId, cancellationToken);
        }
        public async Task<Aula?> ObterAulaPorCursoIdEAulaId(Guid cursoId, Guid aulaId, CancellationToken cancellationToken)
        {
            return await _context.Aulas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == aulaId && a.CursoId == cursoId, cancellationToken);
        }

        public async Task<Curso?> ObterPorNome(string nome, CancellationToken cancellationToken)
        {
            return await _context.Cursos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Nome == nome, cancellationToken);
        }
        public async Task<IEnumerable<Curso>> ObterTodos(CancellationToken cancellationToken)
        {
            return await _context.Cursos
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Curso?> ObterComAulasPorId(Guid cursoId, CancellationToken cancellationToken)
        {
            return await _context.Cursos
                .Include(c => c.Aulas)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cursoId, cancellationToken);
        }
        public async Task InserirAula(Aula aula, CancellationToken cancellationToken)
        {
            await _context.Aulas.AddAsync(aula, cancellationToken);
        }
        public async Task<IEnumerable<Curso>> ObterDisponiveisComAula(CancellationToken cancellationToken)
        {
            return await _context.Cursos
                .Include(c => c.Aulas)
                .AsNoTracking()
                .Where(c => c.Disponivel)
                .ToListAsync(cancellationToken);
        }
        public void Dispose()
        {

        }
    }
}
