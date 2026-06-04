namespace PlataformaEducacao.GestaoIdentidade.Api.Models
{
    public class RefreshToken
    {
        public RefreshToken()
        {
            Id = Guid.NewGuid();
            Token = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid Token { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
