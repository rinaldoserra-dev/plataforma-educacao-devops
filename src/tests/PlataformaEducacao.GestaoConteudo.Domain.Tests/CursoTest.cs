using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;

namespace PlataformaEducacao.GestaoConteudo.Domain.Tests
{
    public class CursoTest
    {
        [Fact(DisplayName = "Adicionar Novo Curso")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void Adicionar_NovoCurso_DeveRetornarCursoValido()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);

            //Act
            var curso = new Curso("Introdução a C#", conteudoProgramatico, 500, true);

            // Assert
            Assert.Equal("Introdução a C#", curso.Nome);
            Assert.Equal(500, curso.Valor);
            Assert.True(curso.Disponivel);
            Assert.Equal(conteudoProgramatico, curso.ConteudoProgramatico);
            Assert.Empty(curso.Aulas);
        }

        [Fact(DisplayName = "Adicionar Novo Curso Com Nome Vazio")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void Adicionar_NovoCurso_DeveRetornarException_Quando_NomeCurso_Vazio()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);

            //Act Assert
            Assert.Throws<DomainException>(() => new Curso("   ", conteudoProgramatico, 500, true));

        }

        [Fact(DisplayName = "Adicionar Novo Curso Com Valor Vazio")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void Adicionar_NovoCurso_DeveRetornarException_Quando_ValorCurso_Vazio()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);

            //Act Assert
            Assert.Throws<DomainException>(() => new Curso("Curso C#", conteudoProgramatico, 0, true));

        }

        [Fact(DisplayName = "Atualizar Nome Curso")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void AtualizarNomeCurso_DeveRetornarCursoValido()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);
            var curso = new Curso("Introdução a C#", conteudoProgramatico, 500, true);

            //Act
            curso.AtualizarNome("Introdução Linguagem C#");

            // Assert
            Assert.Equal("Introdução Linguagem C#", curso.Nome);
        }

        [Fact(DisplayName = "Atualizar Nome Curso Com Valor Inválido")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void AtualizarNomeCurso_DeveRetornarException_Quando_ValorInvalido()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);
            var cursoInvalido = new Curso("Introdução a C#", conteudoProgramatico, 500, true);

            //Act Assert
            Assert.Throws<DomainException>(() => cursoInvalido.AtualizarNome("    "));

        }

        [Fact(DisplayName = "Atualizar Preco Curso Com Valor Inválido")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void AtualizarPrecoCurso_DeveRetornarException_Quando_ValorInvalido()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);
            var cursoInvalido = new Curso("Introdução a C#", conteudoProgramatico, 500, true);

            //Act Assert
            Assert.Throws<DomainException>(() => cursoInvalido.AtualizarValor(-500));

        }

        [Fact(DisplayName = "Tornar Indisponivel Curso")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void TornarIndisponivelCurso()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);
            var curso = new Curso("Introdução a C#", conteudoProgramatico, 500, true);

            //Act
            curso.TornarIndisponivel();

            // Assert
            Assert.False(curso.Disponivel);

        }
        [Fact(DisplayName = "Tornar Disponivel Curso")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void TornarDisponivelCurso()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);
            var curso = new Curso("Introdução a C#", conteudoProgramatico, 500, false);

            //Act
            curso.TornarDisponivel();

            // Assert
            Assert.True(curso.Disponivel);

        }

        [Fact(DisplayName = "Adicionar Aula Válida Ao Curso")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void AdicionarAulaCurso_DeveRetornar_AulaValida()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);
            var curso = new Curso("Introdução a C#", conteudoProgramatico, 500, true);

            var tituloValido = "Estrutura de Dados";
            var conteudoValido = "Conteudo da aula";
            var ordemValida = 1;
            var materialValida = "Material da Aula";
            var aula = new Aula(tituloValido, conteudoValido, ordemValida, materialValida);

            // Act 
            curso.AdicionarAula(aula);

            //Assert
            Assert.Single(curso.Aulas);
        }

        [Fact(DisplayName = "Adicionar Nova Aula Ao Curso Que Já Possui Aula")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void AdicionarAulaCurso_DeveRetornar_Valido_QuandoCursoPossuirAulaJaCadastradas()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);
            var curso = new Curso("Introdução a C#", conteudoProgramatico, 500, true);

            var tituloValido = "Estrutura de Dados";
            var conteudoValido = "Conteudo da aula";
            var ordemValida = 1;
            var materialValida = "Material da Aula";
            var aula = new Aula(tituloValido, conteudoValido, ordemValida, materialValida);
            curso.AdicionarAula(aula);

            var novoTituloValido = "Estrutura de Dados II";
            var novocConteudoValido = "Conteudo da nova aula";
            var novaOrdemValida = 2;
            var novoMaterialValido = "Material da Aula";
            var novaAula = new Aula(novoTituloValido, novocConteudoValido, novaOrdemValida, novoMaterialValido);

            // Act 
            curso.AdicionarAula(novaAula);

            //Assert
            Assert.Equal(2, curso.Aulas.Count);
        }

        [Fact(DisplayName = "Adicionar Aula Existente ao Curso")]
        [Trait("Categoria", "Gestao Conteudo - Curso")]
        public void AdicionarAulaCurso_DeveRetornar_Exception_QuandoTituloAulaExistir()
        {
            // Arrange
            var conteudoProgramatico = new ConteudoProgramatico("Módulo do curso C#", 150);
            var curso = new Curso("Introdução a C#", conteudoProgramatico, 500, true);

            var tituloValido = "Estrutura de Dados";
            var conteudoValido = "Conteudo da aula";
            var ordemValida = 1;
            var materialValido = "Material da Aula";
            var aula = new Aula(tituloValido, conteudoValido, ordemValida, materialValido);

            // Act 
            curso.AdicionarAula(aula);

            //Assert
            Assert.Throws<DomainException>(() => curso.AdicionarAula(aula));
            Assert.Single(curso.Aulas);
        }
    }
}
