using PlataformaEducacao.GestaoFinanceira.Api.Models.Requests;
using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.Models
{
    public class PagarMatriculaRequestTest
    {
        [Fact(DisplayName = "PagarMatriculaRequest válido é considerado válido")]
        [Trait("Categoria", "Gestão Financeira - Requests - PagarMatriculaRequest")]
        public void PagarMatriculaRequest_Valido_RetornaValido()
        {
            // Arrange
            var request = new PagarMatriculaRequest
            {
                MatriculaId = Guid.NewGuid(),
                AlunoId = Guid.NewGuid(),
                Valor = 100m,
                NomeCartao = "Fulano",
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = "123"
            };
            var contexto = new ValidationContext(request);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(request, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.True(ehValido);
            Assert.Empty(resultados);
        }

        [Fact(DisplayName = "PagarMatriculaRequest sem nome do cartão retorna erro")]
        [Trait("Categoria", "Gestão Financeira - Requests - PagarMatriculaRequest")]
        public void PagarMatriculaRequest_SemNomeCartao_RetornaErro()
        {
            // Arrange
            var request = new PagarMatriculaRequest
            {
                MatriculaId = Guid.NewGuid(),
                Valor = 100m,
                NomeCartao = null!,
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = "123"
            };
            var contexto = new ValidationContext(request);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(request, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }

        [Fact(DisplayName = "PagarMatriculaRequest com valor zero retorna erro")]
        [Trait("Categoria", "Gestão Financeira - Requests - PagarMatriculaRequest")]
        public void PagarMatriculaRequest_ValorZero_RetornaErro()
        {
            // Arrange
            var request = new PagarMatriculaRequest
            {
                MatriculaId = Guid.NewGuid(),
                Valor = 0m,
                NomeCartao = "Fulano",
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = "123"
            };
            var contexto = new ValidationContext(request);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(request, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }

        [Fact(DisplayName = "PagarMatriculaRequest sem CVV retorna erro")]
        [Trait("Categoria", "Gestão Financeira - Requests - PagarMatriculaRequest")]
        public void PagarMatriculaRequest_SemCvv_RetornaErro()
        {
            // Arrange
            var request = new PagarMatriculaRequest
            {
                MatriculaId = Guid.NewGuid(),
                Valor = 100m,
                NomeCartao = "Fulano",
                NumeroCartao = "4111111111111111",
                ExpiracaoCartao = "12/2030",
                CvvCartao = null!
            };
            var contexto = new ValidationContext(request);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(request, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
        }
    }
}
