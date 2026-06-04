using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.Bff.Api.Models.Request.GestaoConteudo
{
    public class AtualizarCursoRequest
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Nome { get; set; } = null!;
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string DescricaoConteudo { get; set; } = null!;
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int CargaHoraria { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public decimal Valor { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public bool Disponivel { get; set; }
    }
}
