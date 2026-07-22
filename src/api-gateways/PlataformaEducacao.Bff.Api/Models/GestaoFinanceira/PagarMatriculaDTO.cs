using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.Bff.Api.Models.GestaoFinanceira
{
    public class PagarMatriculaDTO
    {
        [Required(ErrorMessage = "O id da matrícula é obrigatório.")]
        public Guid MatriculaId { get; set; }

        public Guid? AlunoId { get; set; }

        [Range(0.01, 9999999, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O nome do cartão é obrigatório.")]
        public string NomeCartao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número do cartão é obrigatório.")]
        public string NumeroCartao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A expiração do cartão é obrigatória.")]
        public string ExpiracaoCartao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CVV é obrigatório.")]
        public string CvvCartao { get; set; } = string.Empty;
    }
}
