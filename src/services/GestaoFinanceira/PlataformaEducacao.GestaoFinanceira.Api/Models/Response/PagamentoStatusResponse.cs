namespace PlataformaEducacao.GestaoFinanceira.Api.Models.Response
{
    public class PagamentoStatusResponse
    {
        public Guid MatriculaId { get; set; }
        public string Status { get; set; } = string.Empty;

    }
}
