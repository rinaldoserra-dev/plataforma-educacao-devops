using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PlataformaEducacao.Bff.Api.Models.Request.Identidade;
using Xunit;

namespace PlataformaEducacao.Bff.Api.Tests.Models.Request.Identidade
{
    public class RefreshTokenRequestTest
    {
        [Fact(DisplayName = "RefreshTokenRequest deve falhar quando RefreshToken vazio")]
        [Trait("Categoria", "Bff.Api - Models - Identidade")]
        public void RefreshTokenRequest_DeveFalhar_QuandoRefreshTokenVazio()
        {
            // Arrange
            var model = new RefreshTokenRequest { RefreshToken = string.Empty };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, context, validationResults, validateAllProperties: true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.ErrorMessage?.Contains("obrigatório") == true);
        }

        [Fact(DisplayName = "RefreshTokenRequest deve ser válido quando RefreshToken preenchido")]
        [Trait("Categoria", "Bff.Api - Models - Identidade")]
        public void RefreshTokenRequest_DeveSerValido_QuandoRefreshTokenPreenchido()
        {
            // Arrange
            var model = new RefreshTokenRequest { RefreshToken = "valid-refresh-token" };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, context, validationResults, validateAllProperties: true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}
