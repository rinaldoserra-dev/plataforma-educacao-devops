namespace PlataformaEducacao.Bff.Api.Models.GestaoConteudo
{
    public class CursoDetalhesDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public bool Disponivel { get; set; }
        public IEnumerable<AulaResumoDTO> Aulas { get; set; } = Enumerable.Empty<AulaResumoDTO>();
    }
}
