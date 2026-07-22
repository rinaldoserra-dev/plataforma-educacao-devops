using PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Queries.ViewModels
{
    public class MatriculaViewModelTest
    {
        [Fact(DisplayName = "MatriculaViewModel construtor deve atribuir propriedades corretamente")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - ViewModels - MatriculaViewModel")]
        public void MatriculaViewModel_Construtor_DeveAtribuirPropriedades()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var nomeAluno = "Aluno MV";
            var cursoId = Guid.NewGuid();
            var nomeCurso = "Curso MV";
            var situacaoMatricula = SituacaoMatricula.Ativa;
            var dataMatricula = DateTime.UtcNow;
            var situacaoCurso = SituacaoCurso.EmAndamento;
            DateTime? dataConclusao = null;
            var progresso = 50.0;
            Guid? certificadoId = null;
            string? codigo = null;

            // Act
            var viewModel = new MatriculaViewModel(matriculaId, alunoId, nomeAluno, cursoId, nomeCurso, situacaoMatricula, dataMatricula, situacaoCurso, dataConclusao, progresso, certificadoId, codigo);
            var viewModel2 = new MatriculaViewModel(matriculaId, alunoId, null, cursoId, nomeCurso, situacaoMatricula, dataMatricula, situacaoCurso, dataConclusao, progresso, certificadoId, codigo);

            // Assert
            Assert.Equal(matriculaId, viewModel.MatriculaId);
            Assert.Equal(alunoId, viewModel.AlunoId);
            Assert.Equal(nomeAluno, viewModel.NomeAluno);
            Assert.Equal(string.Empty, viewModel2.NomeAluno); // NomeAluno deve ser vazio se for nulo
            Assert.Equal(cursoId, viewModel.CursoId);
            Assert.Equal(nomeCurso, viewModel.NomeCurso);
            Assert.Equal(situacaoMatricula, viewModel.SituacaoMatricula);
            Assert.Equal(dataMatricula, viewModel.DataMatricula);
            Assert.Equal(situacaoCurso, viewModel.SituacaoCurso);
            Assert.Equal(dataConclusao, viewModel.DataConclusao);
            Assert.Equal(progresso, viewModel.ProgressoGeralCurso);
            Assert.Equal(certificadoId, viewModel.CertificadoId);
            Assert.Equal(codigo, viewModel.CodigoVerificacao);
        }

        [Fact(DisplayName = "MatriculaViewModel.FromMatricula deve mapear corretamente a partir da Matricula")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - ViewModels - MatriculaViewModel")]
        public void MatriculaViewModel_FromMatricula_DeveMapearCorretamente()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno MV", "email@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComPagamentoAprovado(Guid.NewGuid(), "Curso MV", totalAulasCurso: 1, valor: 100m, aluno);

            // Act
            var vm = MatriculaViewModel.FromMatricula(matricula);

            // Assert
            Assert.Equal(matricula.Id, vm.MatriculaId);
            Assert.Equal(matricula.AlunoId, vm.AlunoId);
            Assert.Equal(aluno.Nome, vm.NomeAluno);
            Assert.Equal(matricula.CursoId, vm.CursoId);
            Assert.Equal(matricula.NomeCurso, vm.NomeCurso);
            Assert.Equal(matricula.SituacaoMatricula, vm.SituacaoMatricula);
            Assert.Equal(matricula.DataMatricula, vm.DataMatricula);
            Assert.Equal(matricula.HistoricoAprendizado.SituacaoCurso, vm.SituacaoCurso);
            Assert.Equal(matricula.HistoricoAprendizado.DataConclusao, vm.DataConclusao);
            Assert.Equal(matricula.HistoricoAprendizado.ProgressoGeralCurso, vm.ProgressoGeralCurso);
        }
    }
}