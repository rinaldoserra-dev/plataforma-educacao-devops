using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests
{
    public class CertificadoTest
    {
        [Fact(DisplayName = "Criar certificado com matriculaId válido deve gerar código")]
        [Trait("Categoria", "Gestão Aluno - Domain - Certificado")]
        public void CriarCertificado_ComMatriculaIdValido_DeveGerarCodigo()
        {
            // Arrange
            var matriculaId = Guid.NewGuid();

            // Act
            var certificado = new Certificado(matriculaId);

            // Assert
            Assert.Equal(matriculaId, certificado.MatriculaId);
            Assert.False(string.IsNullOrWhiteSpace(certificado.CodigoVerificacao));
        }

        [Fact(DisplayName = "Criar certificado com matriculaId vazio deve lançar DomainException")]
        [Trait("Categoria", "Gestão Aluno - Domain - Certificado")]
        public void CriarCertificado_ComMatriculaIdVazio_DeveLancarDomainException()
        {
            // Act & Assert
            Assert.Throws<DomainException>(() => new Certificado(Guid.Empty));
        }

        [Fact(DisplayName = "CriarCompleto com dados válidos deve criar certificado completo")]
        [Trait("Categoria", "Gestão Aluno - Domain - Certificado")]
        public void CriarCompleto_ComMatriculaECodigoValido_DeveCriar()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso Teste", totalAulasCurso: 5, valor: 100m);
            var codigo = "ABC-123";

            // Act
            var certificado = Certificado.CertificadoFactory.CriarCompleto(matricula, codigo);

            // Assert
            Assert.Equal(matricula.Id, certificado.MatriculaId);
            Assert.Equal(matricula, certificado.Matricula);
            Assert.Equal(codigo, certificado.CodigoVerificacao);
        }

        [Fact(DisplayName = "CriarCompleto com código vazio deve lançar DomainException")]
        [Trait("Categoria", "Gestão Aluno - Domain - Certificado")]
        public void CriarCompleto_ComCodigoVazio_DeveLancarDomainException()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso Teste", totalAulasCurso: 5, valor: 100m);

            // Act & Assert
            Assert.Throws<DomainException>(() => Certificado.CertificadoFactory.CriarCompleto(matricula, string.Empty));
        }
    }
}