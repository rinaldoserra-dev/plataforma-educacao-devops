using PlataformaEducacao.Core.Data;

namespace PlataformaEducacao.GestaoConteudo.Domain
{
    public interface ICursoRepository : IRepository<Curso>
    {
        public Task Inserir(Curso curso, CancellationToken cancellationToken);
        public Task Atualizar(Curso curso, CancellationToken cancellationToken);
        Task<Curso?> ObterPorId(Guid cursoId, CancellationToken cancellationToken);
        Task<Curso?> ObterPorNome(string nome, CancellationToken cancellationToken);
        Task<Curso?> ObterComAulasPorId(Guid cursoId, CancellationToken cancellationToken);
        public Task InserirAula(Aula aula, CancellationToken cancellationToken);
        Task<IEnumerable<Curso>> ObterTodos(CancellationToken cancellationToken);
        Task<IEnumerable<Curso>> ObterDisponiveisComAula(CancellationToken cancellationToken);
        Task<Aula?> ObterAulaPorCursoIdEAulaId(Guid cursoId, Guid aulaId, CancellationToken cancellationToken);

    }
}
