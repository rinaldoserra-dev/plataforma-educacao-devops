namespace PlataformaEducacao.GestaoAluno.Domain.Tests
{
    public class SituacaoMatriculaTest
    {
        [Fact(DisplayName = "SituacaoMatricula deve conter os valores esperados")]
        [Trait("Categoria", "Gestão Aluno - Domain - SituacaoMatricula")]
        public void SituacaoMatricula_ValoresEsperados()
        {
            // Arrange & Act
            var names = Enum.GetNames(typeof(SituacaoMatricula));

            // Assert
            Assert.Contains("PendentePagamento", names);
            Assert.Contains("ProcessoPagamento", names);
            Assert.Contains("Ativa", names);
        }

        [Fact(DisplayName = "SituacaoMatricula possui valores inteiros esperados")]
        [Trait("Categoria", "Gestão Aluno - Domain - SituacaoMatricula")]
        public void SituacaoMatricula_ValoresInteiros()
        {
            // Assert
            Assert.Equal(0, (int)SituacaoMatricula.PendentePagamento);
            Assert.Equal(1, (int)SituacaoMatricula.ProcessoPagamento);
            Assert.Equal(2, (int)SituacaoMatricula.Ativa);
        }

        [Fact(DisplayName = "Parse de string para SituacaoMatricula deve funcionar")]
        [Trait("Categoria", "Gestão Aluno - Domain - SituacaoMatricula")]
        public void SituacaoMatricula_ParseDeString_DeveFuncionar()
        {
            // Act
            var parsed = Enum.Parse<SituacaoMatricula>("Ativa");
            var fromString = (SituacaoMatricula)Enum.Parse(typeof(SituacaoMatricula), "ProcessoPagamento");

            // Assert
            Assert.Equal(SituacaoMatricula.Ativa, parsed);
            Assert.Equal(SituacaoMatricula.ProcessoPagamento, fromString);
        }

        [Fact(DisplayName = "ToString de SituacaoMatricula deve retornar nome do valor")]
        [Trait("Categoria", "Gestão Aluno - Domain - SituacaoMatricula")]
        public void SituacaoMatricula_ToString_DeveRetornarNome()
        {
            // Assert
            Assert.Equal("PendentePagamento", SituacaoMatricula.PendentePagamento.ToString());
            Assert.Equal("ProcessoPagamento", SituacaoMatricula.ProcessoPagamento.ToString());
            Assert.Equal("Ativa", SituacaoMatricula.Ativa.ToString());
        }
    }
}