using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests
{
    public class ProgressoAulaTest
    {
        [Fact(DisplayName = "Criar progresso de aula com aulaId válido deve inicializar propriedades")]
        [Trait("Categoria", "Gestão Aluno - Domain - ProgressoAula")]
        public void CriarProgressoAula_ComAulaIdValido_DeveInicializar()
        {
            // Arrange
            var aulaId = Guid.NewGuid();

            // Act
            var progresso = new ProgressoAula(aulaId);

            // Assert
            Assert.Equal(aulaId, progresso.AulaId);
            Assert.True(progresso.DataConclusao != default);
        }

        [Fact(DisplayName = "Criar progresso de aula com aulaId vazio deve lançar DomainException")]
        [Trait("Categoria", "Gestão Aluno - Domain - ProgressoAula")]
        public void CriarProgressoAula_ComAulaIdVazio_DeveLancarDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new ProgressoAula(Guid.Empty));
        }

        [Fact(DisplayName = "Associar matrícula com id válido deve definir MatriculaId")]
        [Trait("Categoria", "Gestão Aluno - Domain - ProgressoAula")]
        public void AssociarMatricula_ComIdValido_DeveDefinirMatriculaId()
        {
            // Arrange
            var progresso = new ProgressoAula(Guid.NewGuid());
            var matriculaId = Guid.NewGuid();

            // Act
            progresso.AssociarMatricula(matriculaId);

            // Assert
            Assert.Equal(matriculaId, progresso.MatriculaId);
        }

        [Fact(DisplayName = "Associar matrícula com id vazio deve lançar DomainException")]
        [Trait("Categoria", "Gestão Aluno - Domain - ProgressoAula")]
        public void AssociarMatricula_ComIdVazio_DeveLancarDomainException()
        {
            // Arrange
            var progresso = new ProgressoAula(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<DomainException>(() => progresso.AssociarMatricula(Guid.Empty));
        }
    }
}