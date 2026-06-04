using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoConteudo.Api.Requests
{
    public class AdicionarAulaRequest
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public Guid CursoId { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Titulo { get; set; } = null!;
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public string Conteudo { get; set; } = null!;
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int Ordem { get; set; }
        public string? Material { get; set; }
    }
}
