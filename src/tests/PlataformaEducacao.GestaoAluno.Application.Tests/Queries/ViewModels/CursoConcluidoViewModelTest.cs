using PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Queries.ViewModels
{
    public class CursoConcluidoViewModelTest
    {
        [Fact(DisplayName = "CursoConcluidoViewModel construtor deve atribuir propriedades")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - ViewModels - CursoConcluidoViewModel")]
        public void CursoConcluidoViewModel_Construtor_DeveAtribuirPropriedades()
        {
            // Arrange
            var nomeCurso = "Curso Teste";
            var dataMatricula = DateTime.UtcNow.AddDays(-10);
            DateTime? dataConclusao = DateTime.UtcNow;

            // Act
            var viewModel = new CursoConcluidoViewModel(nomeCurso, dataMatricula, dataConclusao);

            // Assert
            Assert.Equal(nomeCurso, viewModel.NomeCurso);
            Assert.Equal(dataMatricula, viewModel.DataMatricula);
            Assert.Equal(dataConclusao, viewModel.DataConclusao);
        }

        [Fact(DisplayName = "CursoConcluidoViewModel.FromMatricula deve mapear corretamente a partir da Matricula")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - ViewModels - CursoConcluidoViewModel")]
        public void CursoConcluidoViewModel_FromMatricula_DeveMapearCorretamente()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno Teste", "email@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso Concluido", totalAulasCurso: 1, valor: 100m, aluno);

            // Act
            var viewModel = CursoConcluidoViewModel.FromMatricula(matricula);

            // Assert
            Assert.Equal(matricula.NomeCurso, viewModel.NomeCurso);
            Assert.Equal(matricula.DataMatricula, viewModel.DataMatricula);
            Assert.Equal(matricula.HistoricoAprendizado.DataConclusao, viewModel.DataConclusao);
        }
    }
}