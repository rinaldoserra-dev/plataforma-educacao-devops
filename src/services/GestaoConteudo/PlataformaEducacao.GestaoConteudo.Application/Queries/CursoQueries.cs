using PlataformaEducacao.GestaoConteudo.Application.Queries.ViewModels;
using PlataformaEducacao.GestaoConteudo.Domain;

namespace PlataformaEducacao.GestaoConteudo.Application.Queries
{
    public class CursoQueries : ICursoQueries
    {
        private readonly ICursoRepository _cursoRepository;

        public CursoQueries(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task<CursoViewModel?> ObterPorId(Guid cursoId, CancellationToken cancellationToken)
        {
            var curso = await _cursoRepository.ObterPorId(cursoId, cancellationToken);

            return curso is null ? null : CursoViewModel.FromCurso(curso);
        }

        public async Task<IEnumerable<CursoViewModel>> ObterTodos(CancellationToken cancellationToken)
        {
            var cursos = await _cursoRepository.ObterTodos(cancellationToken);

            return cursos.Select(CursoViewModel.FromCurso);
        }

        public async Task<IEnumerable<CursoViewModel>> ObterDisponiveisComAula(CancellationToken cancellationToken)
        {
            var cursos = await _cursoRepository.ObterDisponiveisComAula(cancellationToken);

            return cursos.Select(CursoViewModel.FromCurso);
        }

        public async Task<IEnumerable<AulaViewModel>> ObterAulasPorCursoId(Guid cursoId, CancellationToken cancellationToken)
        {
            var curso = await _cursoRepository.ObterComAulasPorId(cursoId, cancellationToken);
            if (curso is null)
            {
                return Enumerable.Empty<AulaViewModel>();
            }

            return curso.Aulas.Select(AulaViewModel.FromAula);
        }
        public async Task<CursoViewModel?> ObterCursoComAulasPorCursoId(Guid cursoId, CancellationToken cancellationToken)
        {
            var curso = await _cursoRepository.ObterComAulasPorId(cursoId, cancellationToken);

            return curso is null ? null : CursoViewModel.FromCurso(curso);
        }
        public async Task<AulaViewModel?> ObterAulaPorCursoIdEAulaId(Guid cursoId, Guid aulaId, CancellationToken cancellationToken)
        {
            var aula = await _cursoRepository.ObterAulaPorCursoIdEAulaId(cursoId, aulaId, cancellationToken);
            return aula is null ? null : AulaViewModel.FromAula(aula);
        }
    }
}
