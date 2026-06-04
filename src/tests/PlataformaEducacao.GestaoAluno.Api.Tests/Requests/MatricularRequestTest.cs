using PlataformaEducacao.GestaoAluno.Api.Requests;
using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoAluno.Api.Tests.Requests
{
    public class MatricularRequestTest
    {
        [Fact(DisplayName = "MatricularRequest válido é considerado válido")]
        [Trait("Categoria", "Gestão Aluno - API - Requests - MatricularRequest")]
        public void MatricularRequest_Valido_RetornaValido()
        {
            // Arrange
            var matricularRequest = new MatricularRequest
            {
                CursoId = Guid.NewGuid(),
                NomeCurso = "Curso de Teste",
                QuantidadeAulasCurso = 10,
                ValorCurso = 150.50m
            };
            var contexto = new ValidationContext(matricularRequest);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(matricularRequest, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.True(ehValido);
            Assert.Empty(resultados);
        }

        [Fact(DisplayName = "Nome do curso vazio resulta em erro de validação")]
        [Trait("Categoria", "Gestão Aluno - API - Requests - MatricularRequest")]
        public void MatricularRequest_NomeCursoVazio_RetornaErroDeValidacao()
        {
            // Arrange
            var matricularRequest = new MatricularRequest
            {
                CursoId = Guid.NewGuid(),
                NomeCurso = string.Empty,
                QuantidadeAulasCurso = 5,
                ValorCurso = 100m
            };
            var contexto = new ValidationContext(matricularRequest);
            var resultados = new List<ValidationResult>();

            // Act
            var ehValido = Validator.TryValidateObject(matricularRequest, contexto, resultados, validateAllProperties: true);

            // Assert
            Assert.False(ehValido);
            Assert.NotEmpty(resultados);
            var mensagens = resultados.Select(r => r.ErrorMessage).ToList();
            Assert.Contains("O nome do curso é obrigatório.", mensagens);
        }
    }
}