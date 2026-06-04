using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels
{
    public class HistoricoAlunoViewModel
    {
        public string NomeAluno { get; set; } = string.Empty;

        public IEnumerable<CursoConcluidoViewModel> CursosConcluidos { get; set; } = new List<CursoConcluidoViewModel>();

        public HistoricoAlunoViewModel(String nomeAluno, IEnumerable<CursoConcluidoViewModel> cursosConcluidos)
        {
            NomeAluno = nomeAluno;
            CursosConcluidos = cursosConcluidos;
        }

        public static HistoricoAlunoViewModel FromAlunoComMatriculas(Aluno alunoComMatriculas)
        => new(
            alunoComMatriculas.Nome,
            alunoComMatriculas.Matriculas
                .Where(m => m.HistoricoAprendizado.SituacaoCurso == SituacaoCurso.Concluido)
                .Select(m => CursoConcluidoViewModel.FromMatricula(m)));
    }
}