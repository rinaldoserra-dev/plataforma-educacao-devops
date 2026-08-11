using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoIdentidade.Api.Models
{

    public class UsuarioClaim
    {
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

}
