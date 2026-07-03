using PlataformaEducacao.GestaoFinanceira.Api.Models.Response;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.Models
{
    public class PagamentoStatusResponseTest
    {
        [Fact(DisplayName = "PagamentoStatusResponse deve atribuir propriedades")]
        [Trait("Categoria", "Gestão Financeira - Response - PagamentoStatusResponse")]
        public void PagamentoStatusResponse_DeveAtribuirPropriedades()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();

            // Act
            var response = new PagamentoStatusResponse
            {
                MatriculaId = matriculaId,
                Status = "Pago"
            };

            // Assert
            Assert.Equal(matriculaId, response.MatriculaId);
            Assert.Equal("Pago", response.Status);
        }
    }
}
