using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoIdentidade.Api.Models
{
    public class UsuarioRefreshToken
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
