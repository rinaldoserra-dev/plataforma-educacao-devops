using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.Bff.Api.Models.GestaoAlunos
{
    public class RealizarAulaDTO
    {
        [Required(ErrorMessage = "O id da matrícula é obrigatório.")]
        public Guid MatriculaId { get; set; }

        [Required(ErrorMessage = "O id do curso é obrigatório.")]
        public Guid CursoId { get; set; }

        [Required(ErrorMessage = "O id da aula é obrigatório.")]
        public Guid AulaId { get; set; }
    }
}