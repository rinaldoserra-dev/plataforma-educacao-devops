using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;

namespace PlataformaEducacao.GestaoConteudo.Domain.Tests
{
    public class ConteudoProgramaticoTest
    {
        [Fact(DisplayName = "Criar Conteudo Programatico Com Descrição Invalida")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void Criar_ConteudoProgramatico_DeveRetornarException_QuandoDescricaoEhVazia()
        {
            // Arrange Act Assert
            Assert.Throws<DomainException>(() => new ConteudoProgramatico("", 150));
        }

        [Fact(DisplayName = "Criar Conteudo Programatico Com Duracao Invalida")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void Criar_ConteudoProgramatico_DeveRetornarException_QuandoDuracaoEhMenorZero()
        {
            // Arrange Act Assert
            Assert.Throws<DomainException>(() => new ConteudoProgramatico("Módulo do curso C#", 0));
        }
    }
}
