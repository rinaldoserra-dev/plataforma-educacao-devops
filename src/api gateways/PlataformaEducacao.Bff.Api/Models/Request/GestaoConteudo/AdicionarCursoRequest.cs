using PlataformaEducacao.WebApi.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.Bff.Api.Models.Request.GestaoConteudo
{
    public class AdicionarCursoRequest
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(255, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(1000, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string DescricaoConteudo { get; set; } = null!;

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(0, int.MaxValue, ErrorMessage = "O campo {0} tem que ser maior que {1}")]
        public int CargaHoraria { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Moeda]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public bool Disponivel { get; set; }
    }
}
