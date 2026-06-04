using FluentAssertions;
using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests.Core
{
    public class EmailTest
    {
        [Fact(DisplayName = "Criar Email com endereço válido deve atribuir Endereco")]
        [Trait("Categoria", "Core - DomainObjects - Email")]
        public void Criar_EnderecoValido_DeveAtribuir()
        {
            // Arrange & Act
            var email = new Email("teste@dominio.com");

            // Assert
            email.Endereco.Should().Be("teste@dominio.com");
        }

        [Fact(DisplayName = "Criar Email com endereço inválido deve lançar DomainException")]
        [Trait("Categoria", "Core - DomainObjects - Email")]
        public void Criar_EnderecoInvalido_DeveLancarDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Email("invalido-sem-arroba"));
        }

        [Fact(DisplayName = "Validar com email válido deve retornar true")]
        [Trait("Categoria", "Core - DomainObjects - Email")]
        public void Validar_EmailValido_DeveRetornarTrue()
        {
            Email.Validar("fulano@teste.com").Should().BeTrue();
        }

        [Fact(DisplayName = "Validar com email inválido deve retornar false")]
        [Trait("Categoria", "Core - DomainObjects - Email")]
        public void Validar_EmailInvalido_DeveRetornarFalse()
        {
            Email.Validar("nao-eh-email").Should().BeFalse();
        }
    }
}
