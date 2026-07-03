using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Business.Tests.Models
{
    public class DadosCartaoTest
    {
        [Fact(DisplayName = "Criar DadosCartao com dados válidos deve criar")]
        [Trait("Categoria", "Gestão Financeira - Business - DadosCartao")]
        public void CriarDadosCartao_ComDadosValidos_DeveCriar()
        {
            // Arrange & Act
            var dados = new DadosCartao("Fulano", "4111111111111111", "12/2030", "123");

            // Assert
            Assert.Equal("Fulano", dados.NomeCartao);
            Assert.Equal("4111111111111111", dados.NumeroCartao);
            Assert.Equal("12/2030", dados.ExpiracaoCartao);
            Assert.Equal("123", dados.CvvCartao);
        }

        [Fact(DisplayName = "Criar DadosCartao sem nome deve lançar DomainException")]
        [Trait("Categoria", "Gestão Financeira - Business - DadosCartao")]
        public void CriarDadosCartao_SemNome_DeveLancarDomainException()
        {
            Assert.Throws<DomainException>(() => new DadosCartao("", "4111111111111111", "12/2030", "123"));
        }

        [Fact(DisplayName = "Criar DadosCartao sem número deve lançar DomainException")]
        [Trait("Categoria", "Gestão Financeira - Business - DadosCartao")]
        public void CriarDadosCartao_SemNumero_DeveLancarDomainException()
        {
            Assert.Throws<DomainException>(() => new DadosCartao("Fulano", "", "12/2030", "123"));
        }

        [Fact(DisplayName = "Criar DadosCartao sem expiração deve lançar DomainException")]
        [Trait("Categoria", "Gestão Financeira - Business - DadosCartao")]
        public void CriarDadosCartao_SemExpiracao_DeveLancarDomainException()
        {
            Assert.Throws<DomainException>(() => new DadosCartao("Fulano", "4111111111111111", "", "123"));
        }

        [Fact(DisplayName = "Criar DadosCartao sem CVV deve lançar DomainException")]
        [Trait("Categoria", "Gestão Financeira - Business - DadosCartao")]
        public void CriarDadosCartao_SemCvv_DeveLancarDomainException()
        {
            Assert.Throws<DomainException>(() => new DadosCartao("Fulano", "4111111111111111", "12/2030", ""));
        }
    }
}
