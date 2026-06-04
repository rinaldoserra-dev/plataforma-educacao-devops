using Moq;
using PlataformaEducacao.GestaoAluno.Application.Queries;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;
using PlataformaEducacao.GestaoAluno.Domain.Services;

namespace PlataformaEducacao.GestaoAluno.Application.Tests.Queries
{
    public class AlunoQueriesTest
    {
        [Fact(DisplayName = "ListarMatriculasPendentesPagamentoPorAlunoId deve retornar DTOs mapeados")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - AlunoQueries")]
        public async Task ListarMatriculasPendentesPagamentoPorAlunoId_RetornaDTOs()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno A", "a@teste.com");
            var matricula = new Matricula(Guid.NewGuid(), "Curso P", totalAulasCurso: 1, valor: 100m);
            matricula.AssociarAluno(aluno.Id);
            var alunoField = typeof(Matricula).GetField("<Aluno>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            alunoField?.SetValue(matricula, aluno);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ListarMatriculasPendentesPagamentoPorAlunoId(aluno.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { matricula });

            var queries = new AlunoQueries(repositorioMock.Object, Mock.Of<ICertificadoService>());

            // Act
            var resultado = await queries.ListarMatriculasPendentesPagamentoPorAlunoId(aluno.Id, CancellationToken.None);

            // Assert
            Assert.Single(resultado);
            var dto = resultado.First();
            Assert.Equal(matricula.Id, dto.MatriculaId);
            Assert.Equal(matricula.NomeCurso, dto.NomeCurso);
        }

        [Fact(DisplayName = "ObterAlunosMatriculadosPorCursoId deve mapear Matriculas para ViewModel")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - AlunoQueries")]
        public async Task ObterAlunosMatriculadosPorCursoId_RetornaViewModels()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno B", "b@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComPagamentoAprovado(Guid.NewGuid(), "Curso M", totalAulasCurso: 1, valor: 50m, aluno);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterAlunosMatriculadosPorCursoId(matricula.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { matricula });

            var queries = new AlunoQueries(repositorioMock.Object, Mock.Of<ICertificadoService>());

            // Act
            var resultado = await queries.ObterAlunosMatriculadosPorCursoId(matricula.CursoId, CancellationToken.None);

            // Assert
            Assert.Single(resultado);
            var viewModel = resultado.First();
            Assert.Equal(matricula.CursoId, viewModel.CursoId);
            Assert.Equal(matricula.NomeCurso, viewModel.NomeCurso);
        }

        [Fact(DisplayName = "ObterAlunosPendentesPorCursoId deve mapear Matriculas para ViewModel")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - AlunoQueries")]
        public async Task ObterAlunosPendentesPorCursoId_RetornaViewModels()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno BP", "bp@teste.com");
            var matricula = new Matricula(Guid.NewGuid(), "Curso MP", totalAulasCurso: 1, valor: 50m);
            matricula.AssociarAluno(aluno.Id);
            var alunoField = typeof(Matricula).GetField("<Aluno>k__BackingField", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            alunoField?.SetValue(matricula, aluno);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterAlunosPendentesPorCursoId(matricula.CursoId, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { matricula });

            var queries = new AlunoQueries(repositorioMock.Object, Mock.Of<ICertificadoService>());

            // Act
            var resultado = await queries.ObterAlunosPendentesPorCursoId(matricula.CursoId, CancellationToken.None);

            // Assert
            Assert.Single(resultado);
            var viewModel = resultado.First();
            Assert.Equal(matricula.CursoId, viewModel.CursoId);
            Assert.Equal(matricula.NomeCurso, viewModel.NomeCurso);
        }

        [Fact(DisplayName = "ObterMatricula retorna ViewModel quando encontrada e null quando não encontrada")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - AlunoQueries")]
        public async Task ObterMatricula_RetornaViewModelOuNull()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno C", "c@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComPagamentoAprovado(Guid.NewGuid(), "Curso X", totalAulasCurso: 1, valor: 50m, aluno);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculaComAlunoPorId(matricula.Id, It.IsAny<CancellationToken>())).ReturnsAsync(matricula);

            var queries = new AlunoQueries(repositorioMock.Object, Mock.Of<ICertificadoService>());

            // Act - primeira parte: matricula encontrada
            var encontrada = await queries.ObterMatricula(matricula.Id, CancellationToken.None);

            // Arrange & Act - segunda parte: matricula não encontrada
            repositorioMock.Setup(r => r.ObterMatriculaComAlunoPorId(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Matricula?)null);
            var naoEncontrada = await queries.ObterMatricula(Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.NotNull(encontrada);
            Assert.Equal(matricula.Id, encontrada!.MatriculaId);

            Assert.Null(naoEncontrada);
        }

        [Fact(DisplayName = "ObterMatriculasAtivasPorAlunoId deve retornar DTOs mapeados")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - AlunoQueries")]
        public async Task ObterMatriculasAtivasPorAlunoId_RetornaDTOs()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), "Aluno D", "d@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComPagamentoAprovado(Guid.NewGuid(), "Curso A", totalAulasCurso: 1, valor: 100m, aluno);

            var repositorioMock = new Mock<IAlunoRepository>();
            repositorioMock.Setup(r => r.ObterMatriculasAtivasPorAlunoId(aluno.Id, It.IsAny<CancellationToken>())).ReturnsAsync(new[] { matricula });

            var queries = new AlunoQueries(repositorioMock.Object, Mock.Of<ICertificadoService>());

            // Act
            var resultado = await queries.ObterMatriculasAtivasPorAlunoId(aluno.Id, CancellationToken.None);

            // Assert
            Assert.Single(resultado);
            var dto = resultado.First();
            Assert.Equal(matricula.Id, dto.MatriculaId);
            Assert.Equal(matricula.NomeCurso, dto.NomeCurso);
        }

        [Fact(DisplayName = "ValidarCertificado retorna null quando não encontrado, null quando matricula sem certificado, e DTO quando encontrado")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - AlunoQueries")]
        public async Task ValidarCertificado_Cases()
        {
            // Arrange
            var repositorioMock = new Mock<IAlunoRepository>();
            var queries = new AlunoQueries(repositorioMock.Object, Mock.Of<ICertificadoService>());

            // Arrange & Act - certificado não encontrado
            repositorioMock.Setup(r => r.ObterCertificadoPorCodigoVerificacao("x", It.IsAny<CancellationToken>())).ReturnsAsync((Matricula?)null);
            var resultado1 = await queries.ValidarCertificado("x", CancellationToken.None);

            // Arrange & Act - matricula encontrada mas sem certificado
            var aluno = new Aluno(Guid.NewGuid(), "Aluno E", "e@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso F", totalAulasCurso: 1, valor: 100m, aluno);
            repositorioMock.Setup(r => r.ObterCertificadoPorCodigoVerificacao("y", It.IsAny<CancellationToken>())).ReturnsAsync(matricula);
            var resultado2 = await queries.ValidarCertificado("y", CancellationToken.None);

            // Arrange & Act - certificado encontrado
            matricula.GerarCertificado();
            repositorioMock.Setup(r => r.ObterCertificadoPorCodigoVerificacao("z", It.IsAny<CancellationToken>())).ReturnsAsync(matricula);
            var resultado3 = await queries.ValidarCertificado("z", CancellationToken.None);

            // Assert
            Assert.Null(resultado1);

            Assert.Null(resultado2);

            Assert.NotNull(resultado3);
            Assert.Equal(matricula.Certificado!.Id, resultado3!.CertificadoId);
        }

        [Fact(DisplayName = "BaixarCertificado retorna null quando não encontrado e ArquivoDTO quando existe")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - AlunoQueries")]
        public async Task BaixarCertificado_Cases()
        {
            // Arrange
            var repositorioMock = new Mock<IAlunoRepository>();
            var servicoMock = new Mock<ICertificadoService>();

            repositorioMock.Setup(r => r.ObterCertificadoPorCertificadoId(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Certificado?)null);
            var queries = new AlunoQueries(repositorioMock.Object, servicoMock.Object);

            // Act - certificado não encontrado
            var resultado1 = await queries.BaixarCertificado(Guid.NewGuid(), CancellationToken.None);

            // Arrange & Act - certificado encontrado
            var aluno = new Aluno(Guid.NewGuid(), "Aluno F", "f@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso Cert", totalAulasCurso: 1, valor: 100m, aluno);
            var certificado = Certificado.CertificadoFactory.CriarCompleto(matricula, "code-123");

            repositorioMock.Setup(r => r.ObterCertificadoPorCertificadoId(certificado.Id, It.IsAny<CancellationToken>())).ReturnsAsync(certificado);
            servicoMock.Setup(s => s.GerarCertificado(certificado)).ReturnsAsync([1, 2, 3]);

            var resultado2 = await queries.BaixarCertificado(certificado.Id, CancellationToken.None);

            // Assert
            Assert.Null(resultado1);

            Assert.NotNull(resultado2);
            Assert.Equal("application/pdf", resultado2!.ContentType);
            var nomeEsperado = $"Certificado_{certificado.Matricula.Aluno.Nome}_{certificado.Matricula.NomeCurso}.pdf".Replace(" ", "_").Replace("/", "-");
            Assert.Equal(nomeEsperado, resultado2.NomeArquivo);
            Assert.Equal(new byte[] { 1, 2, 3 }, resultado2.PdfBytes);
        }

        [Fact(DisplayName = "ObterHistoricoAluno retorna null quando aluno não existe e mapeia quando existe")]
        [Trait("Categoria", "Gestão Aluno - Application - Queries - AlunoQueries")]
        public async Task ObterHistoricoAluno_Cases()
        {
            // Arrange
            var repositorioMock = new Mock<IAlunoRepository>();
            var queries = new AlunoQueries(repositorioMock.Object, Mock.Of<ICertificadoService>());

            repositorioMock.Setup(r => r.ObterComMatriculasPorId(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Aluno?)null);

            // Act - aluno não encontrado
            var resultado1 = await queries.ObterHistoricoAluno(Guid.NewGuid(), CancellationToken.None);

            // Arrange & Act - aluno encontrado
            var aluno = new Aluno(Guid.NewGuid(), "Aluno G", "g@teste.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), "Curso H", totalAulasCurso: 1, valor: 100m, aluno);
            var matriculasFieldAluno = typeof(Aluno).GetField("_matriculas", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var lista = (System.Collections.Generic.List<Matricula>?)matriculasFieldAluno?.GetValue(aluno);
            lista?.Add(matricula);

            repositorioMock.Setup(r => r.ObterComMatriculasPorId(aluno.Id, It.IsAny<CancellationToken>())).ReturnsAsync(aluno);

            var resultado2 = await queries.ObterHistoricoAluno(aluno.Id, CancellationToken.None);

            // Assert
            Assert.Null(resultado1);

            Assert.NotNull(resultado2);
            Assert.Equal(aluno.Nome, resultado2!.NomeAluno);
        }
    }
}