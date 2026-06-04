using PlataformaEducacao.GestaoAluno.Domain.Events;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests.Events
{
    public class CursoFinalizadoEventTest
    {
        [Fact(DisplayName = "CursoFinalizadoEvent deve preencher propriedades corretamente")]
        [Trait("Categoria", "Gestão Aluno - Domain - Events - CursoFinalizadoEvent")]
        public void CursoFinalizadoEvent_Criar_DevePreencherPropriedades()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();

            // Act
            var evento = new CursoFinalizadoEvent(matriculaId);

            // Assert
            Assert.Equal(matriculaId, evento.MatriculaId);
            Assert.Equal(matriculaId, evento.AggregateId);
            Assert.Equal(nameof(CursoFinalizadoEvent), evento.MessageType);
            Assert.True(evento.Timestamp != default);
            Assert.True(evento.Timestamp <= DateTime.Now);
            Assert.True(evento.Timestamp > DateTime.Now.AddSeconds(-5));
        }
    }
}