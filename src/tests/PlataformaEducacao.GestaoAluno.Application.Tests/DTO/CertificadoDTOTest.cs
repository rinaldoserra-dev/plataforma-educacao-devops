using PlataformaEducacao.GestaoAluno.Application.DTO;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.DTO
{
    public class CertificadoDTOTest
    {
        [Fact(DisplayName = "CertificadoDTO deve recuperar valores atribuídos via construtor")]
        [Trait("Categoria", "Gestão Aluno - Application - DTO - CertificadoDTO")]
        public void CertificadoDTO_Construtor_DeveAtribuirPropriedades()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomeAluno = "Aluno Teste";
            var nomeCurso = "Curso X";
            var dataConclusao = DateTime.UtcNow.Date;
            var codigo = "ABC-123";

            // Act
            var dto = new CertificadoDTO(id, nomeAluno, nomeCurso, dataConclusao, codigo);

            // Assert
            Assert.Equal(id, dto.CertificadoId);
            Assert.Equal(nomeAluno, dto.NomeAluno);
            Assert.Equal(nomeCurso, dto.NomeCurso);
            Assert.Equal(dataConclusao, dto.DataConclusao);
            Assert.Equal(codigo, dto.CodigoVerificacao);
        }

        [Fact(DisplayName = "CertificadoDTO.FromMatricula deve mapear corretamente a partir da matrícula")]
        [Trait("Categoria", "Gestão Aluno - Application - DTO - CertificadoDTO")]
        public void CertificadoDTO_FromMatricula_DeveMapearCorretamente()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno Teste", "email@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso X", totalAulasCurso: 1, valor: 100m, aluno);

            // garantir que existe certificado gerado
            matricula.GerarCertificado();

            // Act
            var dto = CertificadoDTO.FromMatricula(matricula);

            // Assert
            Assert.Equal(matricula.Certificado!.Id, dto.CertificadoId);
            Assert.Equal(aluno.Nome, dto.NomeAluno);
            Assert.Equal(matricula.NomeCurso, dto.NomeCurso);
            Assert.Equal(matricula.HistoricoAprendizado.DataConclusao!.Value, dto.DataConclusao);
            Assert.Equal(matricula.Certificado!.CodigoVerificacao, dto.CodigoVerificacao);
        }
    }
}