using PlataformaEducacao.Core.Messages.Integration;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests
{
    public class IntegrationEventsTest
    {
        [Fact(DisplayName = "IniciaPagamentoIntegrationEvent deve preencher propriedades")]
        [Trait("Categoria", "Core - Messages - IniciaPagamentoIntegrationEvent")]
        public void IniciaPagamentoIntegrationEvent_DevePreencherPropriedades()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();

            // Act
            var evento = new IniciaPagamentoIntegrationEvent(
                matriculaId, alunoId, 500m, 1,
                "Fulano", "4111111111111111", "12/2030", "123");

            // Assert
            Assert.Equal(matriculaId, evento.MatriculaId);
            Assert.Equal(alunoId, evento.AlunoId);
            Assert.Equal(500m, evento.Valor);
            Assert.Equal(1, evento.TipoPagamento);
            Assert.Equal("Fulano", evento.NomeCartao);
            Assert.Equal("4111111111111111", evento.NumeroCartao);
            Assert.Equal("12/2030", evento.ExpiracaoCartao);
            Assert.Equal("123", evento.CvvCartao);
            Assert.Equal(matriculaId, evento.AggregateId);
        }

        [Fact(DisplayName = "MatriculaPagamentoRealizadoIntegrationEvent deve preencher propriedades")]
        [Trait("Categoria", "Core - Messages - MatriculaPagamentoRealizadoIntegrationEvent")]
        public void MatriculaPagamentoRealizadoIntegrationEvent_DevePreencherPropriedades()
        {
            var matriculaId = Guid.NewGuid();

            var evento = new MatriculaPagamentoRealizadoIntegrationEvent(matriculaId);

            Assert.Equal(matriculaId, evento.MatriculaId);
            Assert.Equal(matriculaId, evento.AggregateId);
        }

        [Fact(DisplayName = "MatriculaPagamentoRecusadoIntegrationEvent deve preencher propriedades")]
        [Trait("Categoria", "Core - Messages - MatriculaPagamentoRecusadoIntegrationEvent")]
        public void MatriculaPagamentoRecusadoIntegrationEvent_DevePreencherPropriedades()
        {
            var matriculaId = Guid.NewGuid();

            var evento = new MatriculaPagamentoRecusadoIntegrationEvent(matriculaId);

            Assert.Equal(matriculaId, evento.MatriculaId);
            Assert.Equal(matriculaId, evento.AggregateId);
        }
    }
}
