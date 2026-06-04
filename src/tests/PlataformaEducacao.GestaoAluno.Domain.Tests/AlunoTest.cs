using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests
{
    public class AlunoTest
    {
        [Fact(DisplayName = "Criar aluno com dados válidos deve criar")]
        [Trait("Categoria", "Gestão Aluno - Domain - Aluno")]
        public void CriarAluno_ComDadosValidos_DeveCriar()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var aluno = new Aluno(id, "João", "joao@exemplo.com");

            // Assert
            Assert.Equal(id, aluno.Id);
            Assert.Equal("João", aluno.Nome);
            Assert.Equal("joao@exemplo.com", aluno.Email.Endereco);
            Assert.Empty(aluno.Matriculas);
        }

        [Fact(DisplayName = "Criar aluno sem nome deve lançar DomainException")]
        [Trait("Categoria", "Gestão Aluno - Domain - Aluno")]
        public void CriarAluno_SemNome_DeveLancarDomainException()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<DomainException>(() => new Aluno(id, nome: string.Empty, "teste@exemplo.com"));
        }

        [Fact(DisplayName = "Realizar matrícula deve associar ao aluno e deixar pendente de pagamento")]
        [Trait("Categoria", "Gestão Aluno - Domain - Aluno")]
        public void RealizarMatricula_Deve_AssociarEDeixarPendenteDePagamento()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Maria", "maria@exemplo.com");
            var cursoId = Guid.NewGuid();
            var matricula = new Matricula(cursoId, nomeCurso: "Curso 1", totalAulasCurso: 5, valor: 100m);

            // Act
            aluno.RealizarMatricula(matricula);

            // Assert
            Assert.Contains(matricula, aluno.Matriculas);
            Assert.Equal(aluno.Id, matricula.AlunoId);
            Assert.Equal(SituacaoMatricula.PendentePagamento, matricula.SituacaoMatricula);
        }

        [Fact(DisplayName = "Realizar matrícula quando já matriculado deve lançar DomainException")]
        [Trait("Categoria", "Gestão Aluno - Domain - Aluno")]
        public void RealizarMatricula_QuandoJaMatriculado_DeveLancarDomainException()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Carlos", "carlos@exemplo.com");
            var cursoId = Guid.NewGuid();
            var matricula1 = new Matricula(cursoId, nomeCurso: "Curso", totalAulasCurso: 5, valor: 100m);
            var matricula2 = new Matricula(cursoId, nomeCurso: "Curso", totalAulasCurso: 5, valor: 100m);

            // Act
            aluno.RealizarMatricula(matricula1);

            // Assert
            Assert.Throws<DomainException>(() => aluno.RealizarMatricula(matricula2));
        }

        [Fact(DisplayName = "Recusar pagamento de matrícula inexistente deve lançar DomainException")]
        [Trait("Categoria", "Gestão Aluno - Domain - Aluno")]
        public void RecusarPagamentoMatricula_QuandoNaoExistir_DeveLancarDomainException()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Ana", "ana@exemplo.com");
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 5, valor: 100m);

            // Act & Assert
            Assert.Throws<DomainException>(() => aluno.RecusarPagamentoMatricula(matricula));
        }

        [Fact(DisplayName = "Concluir pagamento de matrícula deve ativar matrícula")]
        [Trait("Categoria", "Gestão Aluno - Domain - Aluno")]
        public void ConcluirPagamentoMatricula_DeveAtivarMatricula()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Paulo", "paulo@exemplo.com");
            var cursoId = Guid.NewGuid();
            var matricula = new Matricula(cursoId, nomeCurso: "Curso", totalAulasCurso: 5, valor: 100m);
            aluno.RealizarMatricula(matricula);

            // Act
            aluno.ConcluirPagamentoMatricula(matricula);

            // Arrange
            Assert.Equal(SituacaoMatricula.Ativa, matricula.SituacaoMatricula);
        }
    }
}