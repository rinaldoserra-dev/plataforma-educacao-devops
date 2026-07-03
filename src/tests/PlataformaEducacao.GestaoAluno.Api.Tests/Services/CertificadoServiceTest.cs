using PlataformaEducacao.GestaoAluno.Data.Services;
using PlataformaEducacao.GestaoAluno.Domain;
using QuestPDF.Infrastructure;

namespace PlataformaEducacao.GestaoAluno.Api.Tests.Services
{
    public class CertificadoServiceTest
    {
        public CertificadoServiceTest()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        [Fact(DisplayName = "GerarCertificado deve retornar bytes de PDF válido")]
        [Trait("Categoria", "GestaoAluno - Data - CertificadoService")]
        public async Task GerarCertificado_DeveRetornarBytesDePdf()
        {
            // Arrange
            var service = new CertificadoService();
            var aluno = new Aluno(Guid.NewGuid(), "João da Silva", "joao@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(
                Guid.NewGuid(), "Curso de Microsserviços", 1, 199.90m, aluno);
            var certificado = Certificado.CertificadoFactory.CriarCompleto(
                matricula, Guid.NewGuid().ToString());

            // Act
            var result = await service.GerarCertificado(certificado);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
            Assert.Equal(0x25, result[0]); // PDF magic byte '%'
            Assert.Equal(0x50, result[1]); // 'P'
            Assert.Equal(0x44, result[2]); // 'D'
            Assert.Equal(0x46, result[3]); // 'F'
        }

        [Fact(DisplayName = "GerarCertificado com aluno sem nome deve gerar PDF sem erro")]
        [Trait("Categoria", "GestaoAluno - Data - CertificadoService")]
        public async Task GerarCertificado_AlunoSemNome_DeveGerarPdf()
        {
            // Arrange
            var service = new CertificadoService();
            var aluno = new Aluno(Guid.NewGuid(), "A", "a@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(
                Guid.NewGuid(), "Curso Básico", 1, 50m, aluno);
            var certificado = Certificado.CertificadoFactory.CriarCompleto(
                matricula, "codigo-verificacao-teste");

            // Act
            var result = await service.GerarCertificado(certificado);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 100);
        }
    }
}
