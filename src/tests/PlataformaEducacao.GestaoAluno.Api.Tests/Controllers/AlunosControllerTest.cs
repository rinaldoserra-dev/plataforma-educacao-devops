using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Core.Communication;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoAluno.Api.Controllers;
using PlataformaEducacao.GestaoAluno.Application.Commands.FinalizarCurso;
using PlataformaEducacao.GestaoAluno.Application.Commands.MatricularAlunoCurso;
using PlataformaEducacao.GestaoAluno.Application.Commands.RealizarAula;
using PlataformaEducacao.GestaoAluno.Application.DTO;
using PlataformaEducacao.GestaoAluno.Application.Queries;
using PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels;
using PlataformaEducacao.WebApi.Core.Usuario;
using System.Net;
using System.Security.Claims;

namespace PlataformaEducacao.GestaoAluno.Api.Tests.Controllers
{
    public class AlunosControllerTest
    {
        [Fact(DisplayName = "Obter matrículas ativas retorna OK com dados")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task ObterMatriculasAtivas_RetornaOkComDados()
        {
            // Arrange
            var consultas = new FakeAlunoQueries
            {
                ObterMatriculasAtivasPorAlunoIdResult =
                [
                    new(matriculaId: Guid.NewGuid(), alunoId: Guid.NewGuid(), nomeAluno: "Aluno", cursoId: Guid.NewGuid(), nomeCurso: "Curso", situacaoMatricula: 0,
                        dataMatricula: DateTime.UtcNow, situacaoCurso: 0, dataConclusao: null, progressoGeralCurso: 0, certificadoId: null, codigoVerificacao: null)
                ]
            };

            var controlador = CriarControlador(consultas);

            // Act
            var resultado = await controlador.ObterMatriculasAtivas(CancellationToken.None);

            // Assert
            var resultadoObjeto = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal((int)HttpStatusCode.OK, resultadoObjeto.StatusCode);
            var resposta = Assert.IsType<ResponseResult>(resultadoObjeto.Value);
            Assert.NotNull(resposta.Data);
        }

        [Fact(DisplayName = "Obter matrículas pendentes de pagamento retorna OK com dados")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task ObterMatriculasPendentesPagamento_RetornaOkComDados()
        {
            // Arrange
            var consultas = new FakeAlunoQueries
            {
                ListarMatriculasPendentesPagamentoPorAlunoIdResult = []
            };

            var controlador = CriarControlador(consultas);

            // Act
            var resultado = await controlador.ObterMatriculasPendentesPagamento(CancellationToken.None);

            // Assert
            var resultadoObjeto = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal((int)HttpStatusCode.OK, resultadoObjeto.StatusCode);
            var resposta = Assert.IsType<ResponseResult>(resultadoObjeto.Value);
            Assert.NotNull(resposta.Data);
        }

        [Fact(DisplayName = "Validar certificado retorna OK com dados")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task ValidarCertificado_RetornaOkComDados()
        {
            // Arrange
            var consultas = new FakeAlunoQueries
            {
                ValidarCertificadoResult = new CertificadoDTO(certificadoId: Guid.NewGuid(), nomeAluno: "Aluno", nomeCurso: "Curso", dataConclusao: DateTime.UtcNow,
                    codigoVerificacao: "ABC123")
            };

            var controlador = CriarControlador(consultas);

            // Act
            var resultado = await controlador.ValidarCertificado(codigoVerificacao: "abc", CancellationToken.None);

            // Assert
            var resultadoObjeto = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal((int)HttpStatusCode.OK, resultadoObjeto.StatusCode);
            var resposta = Assert.IsType<ResponseResult>(resultadoObjeto.Value);
            Assert.NotNull(resposta.Data);
        }

        [Fact(DisplayName = "Baixar certificado não encontrado retorna BadRequest")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task BaixarCertificado_NaoEncontrado_RetornaBadRequest()
        {
            // Arrange
            var consultas = new FakeAlunoQueries
            {
                BaixarCertificadoResult = null
            };

            var controlador = CriarControlador(consultas);

            // Act
            var resultado = await controlador.BaixarCertificado(certificadoId: Guid.NewGuid(), CancellationToken.None);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(bad.Value);
            Assert.False(resposta.Sucesso);
            Assert.Contains("Certificado não encontrado.", resposta.Erros.Mensagens);
        }

        [Fact(DisplayName = "Baixar certificado encontrado retorna arquivo")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task BaixarCertificado_Encontrado_RetornaArquivo()
        {
            // Arrange
            var arquivo = new ArquivoDTO { PdfBytes = [1, 2, 3], ContentType = "application/pdf", NomeArquivo = "cert.pdf" };
            var consultas = new FakeAlunoQueries
            {
                BaixarCertificadoResult = arquivo
            };

            var controlador = CriarControlador(consultas);

            // Act
            var resultado = await controlador.BaixarCertificado(Guid.NewGuid(), CancellationToken.None);

            // Assert
            var resultadoArquivo = Assert.IsType<FileContentResult>(resultado);
            Assert.Equal(arquivo.ContentType, resultadoArquivo.ContentType);
        }

        [Fact(DisplayName = "Obter histórico de aluno não encontrado retorna BadRequest")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task ObterHistoricoAluno_NaoEncontrado_RetornaBadRequest()
        {
            // Arrange
            var consultas = new FakeAlunoQueries
            {
                ObterHistoricoAlunoResult = null
            };

            var controlador = CriarControlador(consultas);

            // Act
            var resultado = await controlador.ObterHistoricoAluno(alunoId: Guid.NewGuid(), CancellationToken.None);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var resposta = Assert.IsType<ResponseResult>(bad.Value);
            Assert.False(resposta.Sucesso);
            Assert.Contains("Histórico de aluno não encontrado.", resposta.Erros.Mensagens);
        }

        [Fact(DisplayName = "Matricular com comando válido retorna OK")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task Matricular_ComandoValido_RetornaOk()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var consultas = new FakeAlunoQueries();
            var mediador = new FakeMediatorHandler { SendCommandResult = new ValidationResult() };
            var usuario = new FakeAspNetUser(usuarioId);

            var controlador = new AlunosController(consultas, usuario, mediador);

            var comando = new MatricularAlunoCursoCommand(cursoId: Guid.NewGuid(), alunoId: Guid.Empty, nomeCurso: "Curso X", totalAulasCurso: 1, valor: 10);

            // Act
            var resultado = await controlador.Matricular(comando, CancellationToken.None);

            // Assert
            var obj = Assert.IsType<ObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(obj.Value);
            Assert.True(resposta.Sucesso);
        }

        [Fact(DisplayName = "Matricular com comando inválido retorna BadRequest")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task Matricular_ComandoInvalido_RetornaBadRequest()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var consultas = new FakeAlunoQueries();
            var mediador = new FakeMediatorHandler { SendCommandResult = new ValidationResult() };
            var usuario = new FakeAspNetUser(usuarioId);

            var controlador = new AlunosController(consultas, usuario, mediador);

            var comando = new MatricularAlunoCursoCommand(cursoId: Guid.NewGuid(), alunoId: Guid.Empty, nomeCurso: string.Empty, totalAulasCurso: 1, valor: 10);

            // Act
            var resultado = await controlador.Matricular(comando, CancellationToken.None);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(bad.Value);
            Assert.False(resposta.Sucesso);
        }

        [Fact(DisplayName = "Realizar aula com comando válido retorna OK")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task RealizarAula_ComandoValido_RetornaOk()
        {
            // Arrange
            var consultas = new FakeAlunoQueries();
            var mediador = new FakeMediatorHandler { SendCommandResult = new ValidationResult() };
            var usuario = new FakeAspNetUser(Guid.NewGuid());

            var controlador = new AlunosController(consultas, usuario, mediador);
            var comando = new RealizarAulaCommand(matriculaId: Guid.NewGuid(), cursoId: Guid.NewGuid(), aulaId: Guid.NewGuid());

            // Act
            var resultado = await controlador.RealizarAula(comando, CancellationToken.None);

            // Assert
            var obj = Assert.IsType<ObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(obj.Value);
            Assert.True(resposta.Sucesso);
        }

        [Fact(DisplayName = "Realizar aula com comando inválido retorna BadRequest")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task RealizarAula_ComandoInvalido_RetornaBadRequest()
        {
            // Arrange
            var consultas = new FakeAlunoQueries();
            var mediador = new FakeMediatorHandler { SendCommandResult = new ValidationResult() };
            var usuario = new FakeAspNetUser(Guid.NewGuid());

            var controlador = new AlunosController(consultas, usuario, mediador);
            var comando = new RealizarAulaCommand(matriculaId: Guid.Empty, cursoId: Guid.Empty, aulaId: Guid.Empty);

            // Act
            var resultado = await controlador.RealizarAula(comando, CancellationToken.None);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(bad.Value);
            Assert.False(resposta.Sucesso);
        }

        [Fact(DisplayName = "Finalizar curso com comando válido retorna OK")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task FinalizarCurso_ComandoValido_RetornaOk()
        {
            // Arrange
            var consultas = new FakeAlunoQueries();
            var mediador = new FakeMediatorHandler { SendCommandResult = new ValidationResult() };
            var usuario = new FakeAspNetUser(Guid.NewGuid());

            var controlador = new AlunosController(consultas, usuario, mediador);
            var comando = new FinalizarCursoCommand(matriculaId: Guid.NewGuid());

            // Act
            var resultado = await controlador.FinalizarCurso(comando, CancellationToken.None);

            // Assert
            var obj = Assert.IsType<ObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(obj.Value);
            Assert.True(resposta.Sucesso);
        }

        [Fact(DisplayName = "Finalizar curso com comando inválido retorna BadRequest")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task FinalizarCurso_ComandoInvalido_RetornaBadRequest()
        {
            // Arrange
            var consultas = new FakeAlunoQueries();
            var mediador = new FakeMediatorHandler { SendCommandResult = new ValidationResult() };
            var usuario = new FakeAspNetUser(Guid.NewGuid());

            var controlador = new AlunosController(consultas, usuario, mediador);
            var comando = new FinalizarCursoCommand(matriculaId: Guid.Empty);

            // Act
            var resultado = await controlador.FinalizarCurso(comando, CancellationToken.None);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(resultado);
            var resposta = Assert.IsType<ResponseResult>(bad.Value);
            Assert.False(resposta.Sucesso);
        }

        [Fact(DisplayName = "Obter histórico de aluno encontrado retorna OK")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task ObterHistoricoAluno_Encontrado_RetornaOk()
        {
            // Arrange
            var alunoId = Guid.NewGuid();
            var consultas = new FakeAlunoQueries
            {
                ObterHistoricoAlunoResult = new HistoricoAlunoViewModel(
                    nomeAluno: "Aluno",
                    cursosConcluidos: [])
            };

            var controlador = CriarControlador(consultas);

            // Act
            var resultado = await controlador.ObterHistoricoAluno(alunoId, CancellationToken.None);

            // Assert
            var resultadoObjeto = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal((int)HttpStatusCode.OK, resultadoObjeto.StatusCode);
            var resposta = Assert.IsType<ResponseResult>(resultadoObjeto.Value);
            Assert.True(resposta.Sucesso);
        }

        [Fact(DisplayName = "Obter histórico de outro aluno retorna BadRequest")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task ObterHistoricoAluno_AlunoDiferente_RetornaBadRequest()
        {
            // Arrange
            var idAlunoSolicitado = Guid.NewGuid();
            var idAlunoAutenticado = Guid.NewGuid(); // diferente do solicitado
            var consultas = new FakeAlunoQueries();
            var mediador = new FakeMediatorHandler { SendCommandResult = new ValidationResult() };
            var usuario = new UsuarioFakeSemPapelAdmin(idAlunoAutenticado);

            var controlador = new AlunosController(consultas, usuario, mediador);

            // Act
            var resultado = await controlador.ObterHistoricoAluno(idAlunoSolicitado, CancellationToken.None);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(resultado.Result);
            var resposta = Assert.IsType<ResponseResult>(bad.Value);
            Assert.False(resposta.Sucesso);
            Assert.Contains("Um aluno não pode obter histórico de outro aluno.", resposta.Erros.Mensagens);
        }

        [Fact(DisplayName = "Obter histórico por Admin retorna Ok")]
        [Trait("Categoria", "Gestão Aluno - API - Controllers - AlunosController")]
        public async Task ObterHistoricoAluno_Admin_RetornaOk()
        {
            // Arrange
            var idAlunoSolicitado = Guid.NewGuid();
            var idAdminAutenticado = Guid.NewGuid(); // diferente do solicitado
            var consultas = new FakeAlunoQueries
            {
                ObterHistoricoAlunoResult = new HistoricoAlunoViewModel(
                    nomeAluno: "Aluno",
                    cursosConcluidos: [])
            };
            var mediador = new FakeMediatorHandler { SendCommandResult = new ValidationResult() };
            var usuario = new UsuarioFakeComPapelAdmin(idAdminAutenticado);

            var controlador = new AlunosController(consultas, usuario, mediador);

            // Act
            var resultado = await controlador.ObterHistoricoAluno(idAlunoSolicitado, CancellationToken.None);

            // Assert
            var resultadoObjeto = Assert.IsType<ObjectResult>(resultado.Result);
            Assert.Equal((int)HttpStatusCode.OK, resultadoObjeto.StatusCode);
            var resposta = Assert.IsType<ResponseResult>(resultadoObjeto.Value);
            Assert.True(resposta.Sucesso);
        }

        private static AlunosController CriarControlador(FakeAlunoQueries consultas)
        {
            var usuario = new FakeAspNetUser(Guid.NewGuid());
            var mediador = new FakeMediatorHandler { SendCommandResult = new ValidationResult() };
            return new AlunosController(consultas, usuario, mediador);
        }

        #region Fakes

        private class FakeAlunoQueries : IAlunoQueries
        {
            public IEnumerable<MatriculaAtivaDTO>? ObterMatriculasAtivasPorAlunoIdResult { get; set; }
            public IEnumerable<MatriculaPendentePagamentoDTO>? ListarMatriculasPendentesPagamentoPorAlunoIdResult { get; set; }
            public CertificadoDTO? ValidarCertificadoResult { get; set; }
            public ArquivoDTO? BaixarCertificadoResult { get; set; }
            public HistoricoAlunoViewModel? ObterHistoricoAlunoResult { get; set; }

            public Task<MatriculaViewModel?> ObterMatricula(Guid matriculaId, CancellationToken cancellationToken) =>
                Task.FromResult<MatriculaViewModel?>(null);
            public Task<IEnumerable<MatriculaPendentePagamentoDTO>> ListarMatriculasPendentesPagamentoPorAlunoId(Guid alunoId, CancellationToken cancellationToken) =>
                Task.FromResult(ListarMatriculasPendentesPagamentoPorAlunoIdResult ?? []);
            public Task<IEnumerable<MatriculaAtivaDTO>> ObterMatriculasAtivasPorAlunoId(Guid alunoId, CancellationToken cancellationToken) =>
                Task.FromResult(ObterMatriculasAtivasPorAlunoIdResult ?? []);
            public Task<IEnumerable<MatriculaViewModel>> ObterAlunosMatriculadosPorCursoId(Guid cursoId, CancellationToken cancellationToken) =>
                Task.FromResult(Enumerable.Empty<MatriculaViewModel>());
            public Task<IEnumerable<MatriculaViewModel>> ObterAlunosPendentesPorCursoId(Guid cursoId, CancellationToken cancellationToken) =>
                Task.FromResult(Enumerable.Empty<MatriculaViewModel>());
            public Task<CertificadoDTO?> ValidarCertificado(string codigoVerificacao, CancellationToken cancellationToken) =>
                Task.FromResult(ValidarCertificadoResult);
            public Task<ArquivoDTO?> BaixarCertificado(Guid certificadoId, CancellationToken cancellationToken) =>
                Task.FromResult(BaixarCertificadoResult);
            public Task<HistoricoAlunoViewModel?> ObterHistoricoAluno(Guid alunoId, CancellationToken cancellationToken) =>
                Task.FromResult(ObterHistoricoAlunoResult);
        }

        private class FakeAspNetUser(Guid id) : IAspNetUser
        {
            private readonly Guid _id = id;
            public string Name => string.Empty;
            public Guid ObterUserId() => _id;
            public string ObterUserEmail() => string.Empty;
            public string ObterUserToken() => string.Empty;
            public string ObterUserRefreshToken() => string.Empty;
            public bool EstaAutenticado() => true;
            public virtual bool PossuiRole(string role) => true;
            public IEnumerable<Claim> ObterClaims() => [];
            public HttpContext ObterHttpContext() => new DefaultHttpContext();
        }

        private class UsuarioFakeSemPapelAdmin(Guid id) : FakeAspNetUser(id)
        {
            public override bool PossuiRole(string role) => role != "ADMIN";
        }

        private class UsuarioFakeComPapelAdmin(Guid id) : FakeAspNetUser(id)
        {
            public override bool PossuiRole(string role) => role == "ADMIN";
        }

        private class FakeMediatorHandler : IMediatorHandler
        {
            public ValidationResult SendCommandResult { get; set; } = new ValidationResult();

            public Task PublishEvent<T>(T evento) where T : Event => Task.CompletedTask;

            public Task<ValidationResult> SendCommand<T>(T comando) where T : Command
            {
                return Task.FromResult(SendCommandResult);
            }
        }

        #endregion
    }
}