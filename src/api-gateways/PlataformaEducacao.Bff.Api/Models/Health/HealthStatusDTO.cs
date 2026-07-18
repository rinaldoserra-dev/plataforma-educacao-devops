namespace PlataformaEducacao.Bff.Api.Models.Health
{
    public class HealthStatusDTO
    {
        public string Servico { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public bool Saudavel { get; set; }
        public int Status { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}
