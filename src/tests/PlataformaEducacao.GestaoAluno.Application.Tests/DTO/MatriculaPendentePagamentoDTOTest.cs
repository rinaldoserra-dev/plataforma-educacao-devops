using PlataformaEducacao.GestaoAluno.Application.DTO;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.DTO
{
    public class MatriculaPendentePagamentoDTOTest
    {
        [Fact(DisplayName = "MatriculaPendentePagamentoDTO deve mapear propriedades do construtor corretamente")]
        [Trait("Categoria", "Gestão Aluno - Application - DTO - MatriculaPendentePagamentoDTO")]
        public void MatriculaPendentePagamentoDTO_Construtor_DeveAtribuirPropriedades()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var nomeAluno = "Aluno Y";
            var cursoId = Guid.NewGuid();
            var nomeCurso = "Curso Y";
            var dataMatricula = DateTime.UtcNow;

            // Act
            var dto = new MatriculaPendentePagamentoDTO(matriculaId, alunoId, nomeAluno, cursoId, nomeCurso, dataMatricula);

            // Assert
            Assert.Equal(matriculaId, dto.MatriculaId);
            Assert.Equal(alunoId, dto.AlunoId);
            Assert.Equal(nomeAluno, dto.NomeAluno);
            Assert.Equal(cursoId, dto.CursoId);
            Assert.Equal(nomeCurso, dto.NomeCurso);
            Assert.Equal(dataMatricula, dto.DataMatricula);
        }

        [Fact(DisplayName = "MatriculaPendentePagamentoDTO.FromMatricula deve mapear corretamente a partir de Matricula pendente")]
        [Trait("Categoria", "Gestão Aluno - Application - DTO - MatriculaPendentePagamentoDTO")]
        public void MatriculaPendentePagamentoDTO_FromMatricula_DeveMapearCorretamente()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno Y", "email@teste.com");
            var matricula = new Matricula(Guid.NewGuid(), "Curso Y", totalAulasCurso: 5, valor: 100m);

            matricula.AssociarAluno(aluno.Id);

            // Associar apenas o Id não define a instância de Aluno; usar reflection para preencher a propriedade Aluno
            var alunoField = typeof(Matricula).GetField("<Aluno>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            alunoField?.SetValue(matricula, aluno);

            // Act
            var dto = MatriculaPendentePagamentoDTO.FromMatricula(matricula);

            // Assert
            Assert.Equal(matricula.Id, dto.MatriculaId);
            Assert.Equal(matricula.AlunoId, dto.AlunoId);
            Assert.Equal(aluno.Nome, dto.NomeAluno);
            Assert.Equal(matricula.CursoId, dto.CursoId);
            Assert.Equal(matricula.NomeCurso, dto.NomeCurso);
            Assert.Equal(matricula.DataMatricula, dto.DataMatricula);
        }
    }
}