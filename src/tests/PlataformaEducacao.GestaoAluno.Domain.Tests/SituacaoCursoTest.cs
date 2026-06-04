namespace PlataformaEducacao.GestaoAluno.Domain.Tests
{
    public class SituacaoCursoTest
    {
        [Fact(DisplayName = "SituacaoCurso deve conter os valores esperados")]
        [Trait("Categoria", "Gestão Aluno - Domain - SituacaoCurso")]
        public void SituacaoCurso_ValoresEsperados()
        {
            // Arrange & Act
            var names = Enum.GetNames(typeof(SituacaoCurso));

            // Assert
            Assert.Contains("NaoIniciado", names);
            Assert.Contains("EmAndamento", names);
            Assert.Contains("Concluido", names);
        }

        [Fact(DisplayName = "SituacaoCurso possui valores inteiros esperados")]
        [Trait("Categoria", "Gestão Aluno - Domain - SituacaoCurso")]
        public void SituacaoCurso_ValoresInteiros()
        {
            // Assert
            Assert.Equal(0, (int)SituacaoCurso.NaoIniciado);
            Assert.Equal(1, (int)SituacaoCurso.EmAndamento);
            Assert.Equal(2, (int)SituacaoCurso.Concluido);
        }

        [Fact(DisplayName = "Parse de string para SituacaoCurso deve funcionar")]
        [Trait("Categoria", "Gestão Aluno - Domain - SituacaoCurso")]
        public void SituacaoCurso_ParseDeString_DeveFuncionar()
        {
            // Act
            var parsed = Enum.Parse<SituacaoCurso>("Concluido");
            var fromString = (SituacaoCurso)Enum.Parse(typeof(SituacaoCurso), "EmAndamento");

            // Assert
            Assert.Equal(SituacaoCurso.Concluido, parsed);
            Assert.Equal(SituacaoCurso.EmAndamento, fromString);
        }

        [Fact(DisplayName = "ToString de SituacaoCurso deve retornar nome do valor")]
        [Trait("Categoria", "Gestão Aluno - Domain - SituacaoCurso")]
        public void SituacaoCurso_ToString_DeveRetornarNome()
        {
            // Assert
            Assert.Equal("NaoIniciado", SituacaoCurso.NaoIniciado.ToString());
            Assert.Equal("EmAndamento", SituacaoCurso.EmAndamento.ToString());
            Assert.Equal("Concluido", SituacaoCurso.Concluido.ToString());
        }
    }
}