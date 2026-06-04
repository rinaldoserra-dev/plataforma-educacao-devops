using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoFinanceira.Api.Models.Requests
{
    public class PagarMatriculaRequest
    {
        [Required]
        public Guid MatriculaId { get; set; }
        public Guid AlunoId { get; set; }
        [Range(0.01, 9999999)]
        public decimal Valor { get; set; }
        [Required]
        [StringLength(150)]
        public string NomeCartao { get; set; } = string.Empty;
        [Required]
        [StringLength(19)]
        public string NumeroCartao { get; set; } = string.Empty;
        [Required]
        [StringLength(7)]
        public string ExpiracaoCartao { get; set; } = string.Empty;
        [Required]
        [StringLength(4)]
        public string CvvCartao { get; set; } = string.Empty;
    }
}
