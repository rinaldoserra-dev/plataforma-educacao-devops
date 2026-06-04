using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.DTO
{
    public class MatriculaPendentePagamentoDTO(Guid matriculaId, Guid alunoId, string? nomeAluno,
        Guid cursoId, string nomeCurso, DateTime dataMatricula)
    {
        public Guid MatriculaId { get; set; } = matriculaId;
        public Guid AlunoId { get; set; } = alunoId;
        public string NomeAluno { get; set; } = nomeAluno ?? string.Empty;
        public Guid CursoId { get; set; } = cursoId;
        public string NomeCurso { get; set; } = nomeCurso;
        public DateTime DataMatricula { get; set; } = dataMatricula;

        public static MatriculaPendentePagamentoDTO FromMatricula(Matricula m)
            => new(m.Id, m.AlunoId, m.Aluno.Nome, m.CursoId, m.NomeCurso, m.DataMatricula);
    }
}