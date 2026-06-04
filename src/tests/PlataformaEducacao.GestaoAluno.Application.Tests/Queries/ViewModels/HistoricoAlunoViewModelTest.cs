using PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Queries.ViewModels
{
    public class HistoricoAlunoViewModelTest
    {
        [Fact(DisplayName = "HistoricoAlunoViewModel construtor deve atribuir propriedades")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - ViewModels - HistoricoAlunoViewModel")]
        public void HistoricoAlunoViewModel_Construtor_DeveAtribuirPropriedades()
        {
            // Arrange
            var nomeAluno = "Aluno Historico";
            var cursos = new[] { new CursoConcluidoViewModel("C1", DateTime.UtcNow.AddDays(-10), DateTime.UtcNow) };

            // Act
            var viewModel = new HistoricoAlunoViewModel(nomeAluno, cursos);

            // Assert
            Assert.Equal(nomeAluno, viewModel.NomeAluno);
            Assert.Equal(cursos, viewModel.CursosConcluidos);
        }

        [Fact(DisplayName = "HistoricoAlunoViewModel.FromAlunoComMatriculas deve incluir apenas cursos concluídos")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - ViewModels - HistoricoAlunoViewModel")]
        public void HistoricoAlunoViewModel_FromAlunoComMatriculas_IncluiApenasConcluidos()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno Historico", "email@teste.com");

            var matriculaConcluida = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso C", totalAulasCurso: 1, valor: 100m, aluno);

            var matriculaPendente = new Matricula(Guid.NewGuid(), "Curso Pendente", totalAulasCurso: 1, valor: 100m);
            matriculaPendente.AssociarAluno(aluno.Id);
            var alunoField = typeof(Matricula).GetField("<Aluno>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            alunoField?.SetValue(matriculaPendente, aluno);

            var matriculasFieldAluno = typeof(Aluno).GetField("_matriculas", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var lista = (System.Collections.Generic.List<Matricula>?)matriculasFieldAluno?.GetValue(aluno);
            if (lista is not null)
            {
                lista.Add(matriculaConcluida);
                lista.Add(matriculaPendente);
            }

            // Act
            var viewModel = HistoricoAlunoViewModel.FromAlunoComMatriculas(aluno);

            // Assert
            Assert.Equal(aluno.Nome, viewModel.NomeAluno);
            Assert.Single(viewModel.CursosConcluidos);
            Assert.Equal("Curso C", viewModel.CursosConcluidos.First().NomeCurso);
        }
    }
}