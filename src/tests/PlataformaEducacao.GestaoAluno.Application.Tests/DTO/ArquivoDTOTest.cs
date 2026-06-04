using PlataformaEducacao.GestaoAluno.Application.DTO;
using System.Text;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.DTO
{
    public class ArquivoDTOTest
    {
        [Fact(DisplayName = "ArquivoDTO deve recuperar mesmos valores atribuídos às propriedades")]
        [Trait("Categoria", "Gestão Aluno - Application - DTO - ArquivoDTO")]
        public void ArquivoDTO_Propriedades_DeveRecuperarValoresAtribuidos()
        {
            // Arrange
            var nome = "arquivo.pdf";
            var contentType = "application/pdf";
            var bytes = Encoding.UTF8.GetBytes("conteúdo de teste");

            var dto = new ArquivoDTO
            {
                NomeArquivo = nome,
                ContentType = contentType,
                PdfBytes = bytes
            };

            // Act & Assert
            Assert.Equal(nome, dto.NomeArquivo);
            Assert.Equal(contentType, dto.ContentType);
            Assert.Equal(bytes, dto.PdfBytes);
        }

        [Fact(DisplayName = "ArquivoDTO serializado para JSON deve preservar valores")]
        [Trait("Categoria", "Gestão Aluno - Application - DTO - ArquivoDTO")]
        public void ArquivoDTO_Serializado_DevePreservarValores()
        {
            // Arrange
            var dto = new ArquivoDTO
            {
                NomeArquivo = "arquivo.pdf",
                ContentType = "application/pdf",
                PdfBytes = Encoding.UTF8.GetBytes("conteúdo de teste")
            };

            // Act
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            var dtoDeserialized = System.Text.Json.JsonSerializer.Deserialize<ArquivoDTO>(json);

            // Assert
            Assert.NotNull(dtoDeserialized);
            Assert.Equal(dto.NomeArquivo, dtoDeserialized!.NomeArquivo);
            Assert.Equal(dto.ContentType, dtoDeserialized.ContentType);
            Assert.Equal(dto.PdfBytes, dtoDeserialized.PdfBytes);
        }
    }
}