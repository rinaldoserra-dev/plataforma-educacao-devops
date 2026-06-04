using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoAluno.Api.Requests
{
    public class MatricularRequest
    {
        [Required(ErrorMessage = "O curso é obrigatório.")]
        public Guid CursoId { get; set; }
        [Required(ErrorMessage = "O nome do curso é obrigatório.")]
        public string NomeCurso { get; set; } = string.Empty;
        [Required(ErrorMessage = "A quantidade de aulas do curso é obrigatório.")]
        public int QuantidadeAulasCurso { get; set; }
        [Required(ErrorMessage = "O valor do curso é obrigatório.")]
        public decimal ValorCurso { get; set; }
    }
}
