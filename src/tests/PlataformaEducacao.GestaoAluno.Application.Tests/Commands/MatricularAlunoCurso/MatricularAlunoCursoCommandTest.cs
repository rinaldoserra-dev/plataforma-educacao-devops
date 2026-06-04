using PlataformaEducacao.GestaoAluno.Application.Commands.MatricularAlunoCurso;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Commands.MatricularAlunoCurso
{
    public class MatricularAlunoCursoCommandTest
    {
        [Fact(DisplayName = "MatricularAlunoCursoCommand quando dados corretos deve ser válido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoCommand")]
        public void MatricularAlunoCursoCommand_QuandoDadosCorretos_EhValido()
        {
            // Arrange
            var comando = new MatricularAlunoCursoCommand(
                cursoId: Guid.NewGuid(), alunoId: Guid.NewGuid(), nomeCurso: "Curso X", totalAulasCurso: 10, valor: 100m);

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.True(resultado);
        }

        [Fact(DisplayName = "MatricularAlunoCursoCommand quando AlunoId vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoCommand")]
        public void MatricularAlunoCursoCommand_QuandoAlunoIdVazio_DeveSerInvalido()
        {
            // Arrange
            var comando = new MatricularAlunoCursoCommand(
                cursoId: Guid.NewGuid(), alunoId: Guid.Empty, nomeCurso: "Curso X", totalAulasCurso: 10, valor: 100m);

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(comando.ValidationResult.Errors, e => e.ErrorMessage == "O id do aluno é obrigatório.");
        }

        [Fact(DisplayName = "MatricularAlunoCursoCommand quando CursoId vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoCommand")]
        public void MatricularAlunoCursoCommand_QuandoCursoIdVazio_DeveSerInvalido()
        {
            // Arrange
            var comando = new MatricularAlunoCursoCommand(
                cursoId: Guid.Empty, alunoId: Guid.NewGuid(), nomeCurso: "Curso X", totalAulasCurso: 10, valor: 100m);

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(comando.ValidationResult.Errors, e => e.ErrorMessage == "O id do curso é obrigatório.");
        }

        [Fact(DisplayName = "MatricularAlunoCursoCommand quando NomeCurso vazio deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoCommand")]
        public void MatricularAlunoCursoCommand_QuandoNomeCursoVazio_DeveSerInvalido()
        {
            // Arrange
            var comando = new MatricularAlunoCursoCommand(
                cursoId: Guid.NewGuid(), alunoId: Guid.NewGuid(), nomeCurso: "", totalAulasCurso: 10, valor: 100m);

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(comando.ValidationResult.Errors, e => e.ErrorMessage == "O nome do curso é obrigatório.");
        }

        [Fact(DisplayName = "MatricularAlunoCursoCommand quando Valor inválido deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoCommand")]
        public void MatricularAlunoCursoCommand_QuandoValorInvalido_DeveSerInvalido()
        {
            // Arrange
            var comando = new MatricularAlunoCursoCommand(
                cursoId: Guid.NewGuid(), alunoId: Guid.NewGuid(), nomeCurso: "Curso X", totalAulasCurso: 10, valor: 0m);

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(comando.ValidationResult.Errors, e => e.ErrorMessage == "O valor do curso deve ser maior que 0.");
        }

        [Fact(DisplayName = "MatricularAlunoCursoCommand quando TotalAulasCurso inválido deve ser inválido")]
        [Trait("Categoria", "Gestão Aluno - Application - Commands - MatricularAlunoCursoCommand")]
        public void MatricularAlunoCursoCommand_QuandoTotalAulasCursoInvalido_DeveSerInvalido()
        {
            // Arrange
            var comando = new MatricularAlunoCursoCommand(
                cursoId: Guid.NewGuid(), alunoId: Guid.NewGuid(), nomeCurso: "Curso X", totalAulasCurso: 0, valor: 100m);

            // Act
            var resultado = comando.EhValido();

            // Assert
            Assert.False(resultado);
            Assert.Contains(comando.ValidationResult.Errors, e => e.ErrorMessage == "O número de aulas do curso é obrigatório.");
        }
    }
}