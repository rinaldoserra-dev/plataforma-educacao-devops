using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.Bff.Api.Models.GestaoAlunos
{
    public class FinalizarCursoDTO
    {
        [Required(ErrorMessage = "O id da matrícula é obrigatório.")]
        public Guid MatriculaId { get; set; }
    }
}
