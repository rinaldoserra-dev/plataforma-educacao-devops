using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels
{
    public class CursoConcluidoViewModel
    {
        public string NomeCurso { get; set; } = string.Empty;
        public DateTime DataMatricula { get; set; }
        public DateTime? DataConclusao { get; set; }

        public CursoConcluidoViewModel(String nomeCurso, DateTime dataMatricula, DateTime? dataConclusao)
        {
            NomeCurso = nomeCurso;
            DataMatricula = dataMatricula;
            DataConclusao = dataConclusao;
        }

        public static CursoConcluidoViewModel FromMatricula(Matricula matricula)
            => new(
                matricula.NomeCurso,
                matricula.DataMatricula,
                matricula.HistoricoAprendizado.DataConclusao);
    }
}