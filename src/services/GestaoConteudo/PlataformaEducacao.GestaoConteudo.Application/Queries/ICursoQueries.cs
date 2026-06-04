using PlataformaEducacao.GestaoConteudo.Application.Queries.ViewModels;

namespace PlataformaEducacao.GestaoConteudo.Application.Queries
{
    public interface ICursoQueries
    {
        Task<CursoViewModel?> ObterPorId(Guid cursoId, CancellationToken cancellationToken);

        Task<IEnumerable<CursoViewModel>> ObterTodos(CancellationToken cancellationToken);

        Task<IEnumerable<CursoViewModel>> ObterDisponiveisComAula(CancellationToken cancellationToken);

        Task<IEnumerable<AulaViewModel>> ObterAulasPorCursoId(Guid cursoId, CancellationToken cancellationToken);
        Task<CursoViewModel?> ObterCursoComAulasPorCursoId(Guid cursoId, CancellationToken cancellationToken);
        Task<AulaViewModel?> ObterAulaPorCursoIdEAulaId(Guid cursoId, Guid aulaId, CancellationToken cancellationToken);
    }
}
