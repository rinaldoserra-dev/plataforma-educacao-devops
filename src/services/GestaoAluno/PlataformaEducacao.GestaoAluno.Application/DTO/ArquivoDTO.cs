namespace PlataformaEducacao.GestaoAluno.Application.DTO
{
    public class ArquivoDTO
    {
        public byte[] PdfBytes { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string NomeArquivo { get; set; } = null!;
    }
}