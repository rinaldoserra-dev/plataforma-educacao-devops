using FluentAssertions;
using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests.Core
{
    public class EntityTest
    {
        private class EntidadeDeTeste : Entity { }

        private class OutraEntidadeDeTeste : Entity { }

        private class EventoDeTeste : Event
        {
            public EventoDeTeste() : base() { }
        }

        [Fact(DisplayName = "Construtor deve gerar Id e inicializar notificações")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void Construtor_DeveGerarIdEInicializarNotificacoes()
        {
            // Arrange & Act
            var entidade = new EntidadeDeTeste();

            // Assert
            entidade.Id.Should().NotBe(Guid.Empty);
            entidade.Notificacoes.Should().NotBeNull().And.BeEmpty();
        }

        [Fact(DisplayName = "DefinirId deve atualizar Id")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void DefinirId_DeveAtualizarId()
        {
            // Arrange
            var entidade = new EntidadeDeTeste();
            var novoId = Guid.NewGuid();

            // Act
            entidade.DefinirId(novoId);

            // Assert
            entidade.Id.Should().Be(novoId);
        }

        [Fact(DisplayName = "AdicionarEvento deve adicionar à lista de notificações")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void AdicionarEvento_DeveAdicionarNaLista()
        {
            // Arrange
            var entidade = new EntidadeDeTeste();
            var evento = new EventoDeTeste();

            // Act
            entidade.AdicionarEvento(evento);

            // Assert
            entidade.Notificacoes.Should().ContainSingle().Which.Should().Be(evento);
        }

        [Fact(DisplayName = "RemoverEvento deve remover da lista de notificações")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void RemoverEvento_DeveRemoverDaLista()
        {
            // Arrange
            var entidade = new EntidadeDeTeste();
            var evento = new EventoDeTeste();
            entidade.AdicionarEvento(evento);

            // Act
            entidade.RemoverEvento(evento);

            // Assert
            entidade.Notificacoes.Should().BeEmpty();
        }

        [Fact(DisplayName = "LimparEventos deve remover todas as notificações")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void LimparEventos_DeveLimparLista()
        {
            // Arrange
            var entidade = new EntidadeDeTeste();
            entidade.AdicionarEvento(new EventoDeTeste());
            entidade.AdicionarEvento(new EventoDeTeste());

            // Act
            entidade.LimparEventos();

            // Assert
            entidade.Notificacoes.Should().BeEmpty();
        }

        [Fact(DisplayName = "Equals com mesma referência deve retornar true")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void Equals_MesmaReferencia_DeveRetornarTrue()
        {
            // Arrange
            var entidade = new EntidadeDeTeste();

            // Act & Assert
            entidade.Equals(entidade).Should().BeTrue();
        }

        [Fact(DisplayName = "Equals com objeto nulo deve retornar false")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void Equals_ObjetoNulo_DeveRetornarFalse()
        {
            // Arrange
            var entidade = new EntidadeDeTeste();

            // Act & Assert
            entidade.Equals(null).Should().BeFalse();
        }

        [Fact(DisplayName = "Equals com entidade de mesmo Id deve retornar true")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void Equals_MesmoId_DeveRetornarTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entidade1 = new EntidadeDeTeste();
            var entidade2 = new EntidadeDeTeste();
            entidade1.DefinirId(id);
            entidade2.DefinirId(id);

            // Act & Assert
            entidade1.Equals(entidade2).Should().BeTrue();
        }

        [Fact(DisplayName = "Equals com entidade de Id diferente deve retornar false")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void Equals_IdsDiferentes_DeveRetornarFalse()
        {
            // Arrange
            var entidade1 = new EntidadeDeTeste();
            var entidade2 = new EntidadeDeTeste();

            // Act & Assert
            entidade1.Equals(entidade2).Should().BeFalse();
        }

        [Fact(DisplayName = "Equals com objeto de outro tipo deve comparar como Entity")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void Equals_OutroTipo_DeveRetornarFalse()
        {
            // Arrange
            var entidade = new EntidadeDeTeste();
            var outroObjeto = new object();

            // Act & Assert
            entidade.Equals(outroObjeto).Should().BeFalse();
        }

        [Fact(DisplayName = "Operator == com ambas referências nulas deve retornar true")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void OperatorIgual_AmbasNulas_DeveRetornarTrue()
        {
            // Arrange
            Entity? a = null;
            Entity? b = null;

            // Act & Assert
            (a == b).Should().BeTrue();
        }

        [Fact(DisplayName = "Operator == com primeira referência nula deve retornar false")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void OperatorIgual_PrimeiraNula_DeveRetornarFalse()
        {
            // Arrange
            Entity? a = null;
            Entity b = new EntidadeDeTeste();

            // Act & Assert
            (a == b).Should().BeFalse();
        }

        [Fact(DisplayName = "Operator == com segunda referência nula deve retornar false")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void OperatorIgual_SegundaNula_DeveRetornarFalse()
        {
            // Arrange
            Entity a = new EntidadeDeTeste();
            Entity? b = null;

            // Act & Assert
            (a == b).Should().BeFalse();
        }

        [Fact(DisplayName = "Operator == com entidades de mesmo Id deve retornar true")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void OperatorIgual_MesmoId_DeveRetornarTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            var a = new EntidadeDeTeste();
            var b = new EntidadeDeTeste();
            a.DefinirId(id);
            b.DefinirId(id);

            // Act & Assert
            (a == b).Should().BeTrue();
        }

        [Fact(DisplayName = "Operator != com entidades de Ids diferentes deve retornar true")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void OperatorDiferente_IdsDiferentes_DeveRetornarTrue()
        {
            // Arrange
            var a = new EntidadeDeTeste();
            var b = new EntidadeDeTeste();

            // Act & Assert
            (a != b).Should().BeTrue();
        }

        [Fact(DisplayName = "Operator != com entidades de mesmo Id deve retornar false")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void OperatorDiferente_MesmoId_DeveRetornarFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var a = new EntidadeDeTeste();
            var b = new EntidadeDeTeste();
            a.DefinirId(id);
            b.DefinirId(id);

            // Act & Assert
            (a != b).Should().BeFalse();
        }

        [Fact(DisplayName = "GetHashCode deve retornar valor consistente com Id e Tipo")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void GetHashCode_DeveRetornarConsistente()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entidade1 = new EntidadeDeTeste();
            var entidade2 = new EntidadeDeTeste();
            entidade1.DefinirId(id);
            entidade2.DefinirId(id);

            // Act & Assert
            entidade1.GetHashCode().Should().Be(entidade2.GetHashCode());
        }

        [Fact(DisplayName = "GetHashCode deve ser diferente para tipos diferentes com mesmo Id")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void GetHashCode_TiposDiferentes_DeveSerDiferente()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entidade1 = new EntidadeDeTeste();
            var entidade2 = new OutraEntidadeDeTeste();
            entidade1.DefinirId(id);
            entidade2.DefinirId(id);

            // Act & Assert
            entidade1.GetHashCode().Should().NotBe(entidade2.GetHashCode());
        }

        [Fact(DisplayName = "ToString deve conter o nome do tipo e o Id")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void ToString_DeveConterTipoEId()
        {
            // Arrange
            var entidade = new EntidadeDeTeste();

            // Act
            var resultado = entidade.ToString();

            // Assert
            resultado.Should().Contain(nameof(EntidadeDeTeste));
            resultado.Should().Contain(entidade.Id.ToString());
        }

        [Fact(DisplayName = "EhValido deve lançar NotImplementedException por padrão")]
        [Trait("Categoria", "Core - DomainObjects - Entity")]
        public void EhValido_DeveLancarNotImplementedException()
        {
            // Arrange
            var entidade = new EntidadeDeTeste();

            // Act & Assert
            Assert.Throws<NotImplementedException>(() => entidade.EhValido());
        }
    }
}
