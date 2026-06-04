using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoConteudo.Domain.Tests
{
    public class AulaTest
    {
        [Fact(DisplayName = "Criar Aula Com titulo Invalido")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void Criar_Aula_DeveRetornarException_QuandoDescricaoEhVazia()
        {
            // Arrange
            var tituloInvalido = "";
            var conteudoValido = "Conteudo da aula";
            var ordemValida = 1;
            var material = "Material da Aula";
            // Act 
            var ex = Assert.Throws<DomainException>(() => new Aula(tituloInvalido, conteudoValido, ordemValida, material));

            //Assert
            Assert.Equal("O título da aula é orbigatório.", ex.Message);
        }

        [Fact(DisplayName = "Criar Aula Com Conteudo Invalido")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void Criar_Aula_ComConteudoInvalido_DeveLancarExcecao()
        {
            // Arrange
            var tituloValido = "Estrutura de Dados";
            var conteudoInvalido = "";
            var ordemValida = 1;
            var material = "Material da Aula";

            // Act 
            var ex = Assert.Throws<DomainException>(() => new Aula(tituloValido, conteudoInvalido, ordemValida, material));

            //Assert
            Assert.Equal("O conteudo da aula é obrigatório.", ex.Message);
        }

        [Fact(DisplayName = "Criar Aula Valida Com Material")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void Criar_Aula_Valida()
        {
            // Arrange
            var tituloValido = "Estrutura de Dados";
            var conteudoValido = "Conteudo da aula";
            var ordemValida = 1;
            var materialValido = "Material da Aula";
            // Act 
            var aula = new Aula(tituloValido, conteudoValido, ordemValida, materialValido);

            //Assert
            Assert.Equal(aula.Titulo, tituloValido);
            Assert.Equal(aula.Conteudo, conteudoValido);
            Assert.Equal(aula.Ordem, ordemValida);
            Assert.Equal(aula.Material, materialValido);
        }

        [Fact(DisplayName = "Criar Aula Valida Sem Material")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void Criar_AulaSemMaterial_Valida()
        {
            // Arrange
            var tituloValido = "Estrutura de Dados";
            var conteudoValido = "Conteudo da aula";
            var ordemValida = 1;

            // Act 
            var aula = new Aula(tituloValido, conteudoValido, ordemValida, null);

            //Assert
            Assert.Equal(aula.Titulo, tituloValido);
            Assert.Equal(aula.Conteudo, conteudoValido);
            Assert.Null(aula.Material);
        }
    }
}
