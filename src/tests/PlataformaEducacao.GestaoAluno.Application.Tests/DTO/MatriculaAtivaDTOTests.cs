using PlataformaEducacao.GestaoAluno.Application.DTO;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.DTO
{
    public class MatriculaAtivaDTOTests
    {
        [Fact(DisplayName = "MatriculaAtivaDTO deve mapear propriedades do construtor corretamente")]
        [Trait("Categoria", "Gestão Aluno - Application - DTO - MatriculaAtivaDTO")]
        public void MatriculaAtivaDTO_Construtor_DeveAtribuirPropriedades()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();
            var alunoId = Guid.NewGuid();
            var nomeAluno = "Aluno X";
            var cursoId = Guid.NewGuid();
            var nomeCurso = "Curso X";
            var situacaoMatricula = 1;
            var dataMatricula = DateTime.UtcNow;
            var situacaoCurso = 2;
            DateTime? dataConclusao = DateTime.UtcNow.Date;
            var progresso = 75.5;
            Guid? certificadoId = Guid.NewGuid();
            string? codigo = "ABC-123";

            // Act
            var dto = new MatriculaAtivaDTO(matriculaId, alunoId, nomeAluno, cursoId, nomeCurso, situacaoMatricula, dataMatricula, situacaoCurso, dataConclusao, progresso, certificadoId, codigo);

            // Assert
            Assert.Equal(matriculaId, dto.MatriculaId);
            Assert.Equal(alunoId, dto.AlunoId);
            Assert.Equal(nomeAluno, dto.NomeAluno);
            Assert.Equal(cursoId, dto.CursoId);
            Assert.Equal(nomeCurso, dto.NomeCurso);
            Assert.Equal(situacaoMatricula, dto.SituacaoMatricula);
            Assert.Equal(dataMatricula, dto.DataMatricula);
            Assert.Equal(situacaoCurso, dto.SituacaoCurso);
            Assert.Equal(dataConclusao, dto.DataConclusao);
            Assert.Equal(progresso, dto.ProgressoGeralCurso);
            Assert.Equal(certificadoId, dto.CertificadoId);
            Assert.Equal(codigo, dto.CodigoVerificacao);
        }

        [Fact(DisplayName = "MatriculaAtivaDTO.FromMatricula deve mapear corretamente a partir de Matricula ativa sem certificado")]
        [Trait("Categoria", "Gestão Aluno - Application - DTO - MatriculaAtivaDTO")]
        public void MatriculaAtivaDTO_FromMatricula_DeveMapearCorretamente()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno X", "email@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso X", totalAulasCurso: 1, valor: 100m, aluno);

            // Act
            var dto = MatriculaAtivaDTO.FromMatricula(matricula);

            // Assert
            Assert.Equal(matricula.Id, dto.MatriculaId);
            Assert.Equal(matricula.AlunoId, dto.AlunoId);
            Assert.Equal(aluno.Nome, dto.NomeAluno);
            Assert.Equal(matricula.CursoId, dto.CursoId);
            Assert.Equal(matricula.NomeCurso, dto.NomeCurso);
            Assert.Equal((int)matricula.SituacaoMatricula, dto.SituacaoMatricula);
            Assert.Equal(matricula.DataMatricula, dto.DataMatricula);
            Assert.Equal((int)matricula.HistoricoAprendizado.SituacaoCurso, dto.SituacaoCurso);
            Assert.Equal(matricula.HistoricoAprendizado.DataConclusao, dto.DataConclusao);
            Assert.Equal(matricula.HistoricoAprendizado.ProgressoGeralCurso, dto.ProgressoGeralCurso);
        }

        [Fact(DisplayName = "MatriculaAtivaDTO.FromMatricula deve mapear  corretamente a partir de Matricula com certificado")]
        [Trait("Categoria", "Gestão Aluno - Application - DTO - MatriculaAtivaDTO")]
        public void MatriculaAtivaDTO_FromMatriculaComCertificado_DeveMapearCorretamente()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno X", "email@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso X", totalAulasCurso: 1, valor: 100m, aluno);
            matricula.GerarCertificado();
            var certificado = matricula.Certificado;

            // Act
            var dto = MatriculaAtivaDTO.FromMatricula(matricula);

            // Assert
            Assert.NotNull(certificado);
            Assert.Equal(certificado.Id, dto.CertificadoId);
            Assert.Equal(certificado.CodigoVerificacao, dto.CodigoVerificacao);
        }
    }
}