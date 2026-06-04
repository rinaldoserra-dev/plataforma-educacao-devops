using PlataformaEducacao.GestaoAluno.Domain.Events;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests.Events
{
    public class MatriculaAtivadaEventTest
    {
        [Fact(DisplayName = "MatriculaAtivadaEvent deve preencher propriedades corretamente")]
        [Trait("Categoria", "Gestão Aluno - Domain - Events - MatriculaAtivadaEvent")]
        public void MatriculaAtivadaEvent_Criar_DevePreencherPropriedades()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();

            // Act
            var evento = new MatriculaAtivadaEvent(matriculaId);

            // Assert
            Assert.Equal(matriculaId, evento.MatriculaId);
            Assert.Equal(matriculaId, evento.AggregateId);
            Assert.Equal(nameof(MatriculaAtivadaEvent), evento.MessageType);
            Assert.True(evento.Timestamp != default);
            Assert.True(evento.Timestamp <= DateTime.Now);
            Assert.True(evento.Timestamp > DateTime.Now.AddSeconds(-5));
        }
    }
}