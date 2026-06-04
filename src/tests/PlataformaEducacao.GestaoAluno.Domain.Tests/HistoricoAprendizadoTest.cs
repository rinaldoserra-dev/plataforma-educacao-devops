using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests
{
    public class HistoricoAprendizadoTest
    {
        [Fact(DisplayName = "CriarInicial deve criar histórico com progresso zero")]
        [Trait("Categoria", "Gestão Aluno - Domain - HistoricoAprendizado")]
        public void CriarInicial_DeveCriarHistoricoPendente()
        {
            // Arrange & Act
            var historico = HistoricoAprendizado.HistoricoAprendizadoFactory.CriarInicial(totalAulasCurso: 10);

            // Assert
            Assert.Equal(10, historico.TotalAulasCurso);
            Assert.Equal(0, historico.ProgressoGeralCurso);
            Assert.Equal(SituacaoCurso.NaoIniciado, historico.SituacaoCurso);
            Assert.Null(historico.DataConclusao);
        }

        [Fact(DisplayName = "CriarEmAndamento deve criar histórico em andamento com progresso informado")]
        [Trait("Categoria", "Gestão Aluno - Domain - HistoricoAprendizado")]
        public void CriarEmAndamento_DeveCriarHistoricoEmAndamento()
        {
            // Arrange & Act
            var historico = HistoricoAprendizado.HistoricoAprendizadoFactory.CriarEmAndamento(totalAulasCurso: 20, progressoGeral: 45.5);

            // Assert
            Assert.Equal(20, historico.TotalAulasCurso);
            Assert.Equal(45.5, historico.ProgressoGeralCurso);
            Assert.Equal(SituacaoCurso.EmAndamento, historico.SituacaoCurso);
            Assert.Null(historico.DataConclusao);
        }

        [Fact(DisplayName = "CriarFinalizado deve criar histórico concluído e preencher DataConclusao")]
        [Trait("Categoria", "Gestão Aluno - Domain - HistoricoAprendizado")]
        public void CriarFinalizado_DeveCriarHistoricoConcluido()
        {
            // Arrange & Act
            var historico = HistoricoAprendizado.HistoricoAprendizadoFactory.CriarFinalizado(totalAulasCurso: 15, progressoGeral: 100);

            // Assert
            Assert.Equal(15, historico.TotalAulasCurso);
            Assert.Equal(100, historico.ProgressoGeralCurso);
            Assert.Equal(SituacaoCurso.Concluido, historico.SituacaoCurso);
            Assert.True(historico.DataConclusao.HasValue);
        }

        [Fact(DisplayName = "Criar com TotalAulasCurso menor ou igual a zero deve lançar DomainException")]
        [Trait("Categoria", "Gestão Aluno - Domain - HistoricoAprendizado")]
        public void Criar_TotalAulasCursoMenorOuIgualZero_DeveLancarDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => HistoricoAprendizado.HistoricoAprendizadoFactory.CriarInicial(totalAulasCurso: 0));
            Assert.Throws<DomainException>(() => HistoricoAprendizado.HistoricoAprendizadoFactory.CriarEmAndamento(totalAulasCurso: 0, progressoGeral: 10));
            Assert.Throws<DomainException>(() => HistoricoAprendizado.HistoricoAprendizadoFactory.CriarFinalizado(totalAulasCurso: 0, progressoGeral: 100));
        }
    }
}