using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.Bff.Api.Models.Request.Identidade
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string RefreshToken { get; set; } = string.Empty;

    }
}
