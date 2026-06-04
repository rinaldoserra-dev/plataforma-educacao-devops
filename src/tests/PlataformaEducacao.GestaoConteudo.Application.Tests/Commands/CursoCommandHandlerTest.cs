using Moq;
using Moq.AutoMock;
using PlataformaEducacao.GestaoConteudo.Application.Commands;
using PlataformaEducacao.GestaoConteudo.Domain;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;

namespace PlataformaEducacao.GestaoConteudo.Application.Tests.Commands
{
    public class CursoCommandHandlerTest
    {
        private readonly AutoMocker _mocker;
        private readonly CursoCommandHandler _handler;

        public CursoCommandHandlerTest()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<CursoCommandHandler>();
        }

        [Fact(DisplayName = "Adicionar curso com Comandos Inválidas")]
        [Trait("Categoria", "Gestao Conteudo - CursoCommandHandler")]
        public async Task AdicionarCurso_DeveRetornarFalso_QuandoOComandoEInvalido()
        {
            var command = new AdicionarCursoCommand("Curso XX", "", 0, 0, true);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsValid);
            _mocker.GetMock<ICursoRepository>()
                .Verify(r => r.Inserir(It.IsAny<Curso>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Adicionar curso com nome já cadastrado")]
        [Trait("Categoria", "Gestao Conteudo - CursoCommandHandler")]
        public async Task AdicionarCurso_DeveRetornarFalso_QuandoONomeCursoJaExiste()
        {
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Descricao", 50), 500, true);

            var command = new AdicionarCursoCommand("Curso C#", "Conteudo", 20, 500, true);
            _mocker.GetMock<ICursoRepository>().Setup(x => x.ObterPorNome(command.Nome, CancellationToken.None)).ReturnsAsync(curso);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Já possui curso com esse nome!");
            _mocker.GetMock<ICursoRepository>()
                .Verify(r => r.Inserir(It.IsAny<Curso>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Adicionar curso válido")]
        [Trait("Categoria", "Gestao Conteudo - CursoCommandHandler")]
        public async Task AdicionarCurso_CommandValido_DeveExecutarComSucesso()
        {
            // Arrange
            var command = new AdicionarCursoCommand("Curso C#", "Conteudo", 20, 500, true);

            _mocker.GetMock<ICursoRepository>().Setup(x => x.ObterPorNome(command.Nome, CancellationToken.None)).ReturnsAsync((Curso)null!);
            _mocker.GetMock<ICursoRepository>().Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.ObterPorNome(command.Nome, CancellationToken.None), Times.Once);
            _mocker.GetMock<ICursoRepository>().Verify(r => r.Inserir(It.IsAny<Curso>(), CancellationToken.None), Times.Once);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Atualizar curso com Id Curso não cadastrado")]
        [Trait("Categoria", "Gestao Conteudo - CursoCommandHandler")]
        public async Task AtualizarCurso_DeveRetornarFalso_QuandoOIdCursoNaoExiste()
        {
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Descricao", 50), 500, true);

            var command = new AtualizarCursoCommand(Guid.NewGuid(), "Curso Liguagem C#", "Conteudo do curso", 20, 500, true);

            _mocker.GetMock<ICursoRepository>().Setup(x => x.ObterPorId(command.CursoId, CancellationToken.None)).ReturnsAsync((Curso?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsValid);
            _mocker.GetMock<ICursoRepository>()
                .Verify(r => r.Atualizar(It.IsAny<Curso>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Atualizar curso válido")]
        [Trait("Categoria", "Gestao Conteudo - CursoCommandHandler")]
        public async Task AtualizarCurso_CommandValido_DeveExecutarComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Conteudo Programatico", 20), 500, true);

            var command = new AtualizarCursoCommand(curso.Id, "Curso Linguagem C#", "Conteudo Programatico", 25, 600, true);

            _mocker.GetMock<ICursoRepository>().Setup(x => x.ObterPorId(command.CursoId, CancellationToken.None)).ReturnsAsync(curso);
            _mocker.GetMock<ICursoRepository>().Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.ObterPorId(command.CursoId, CancellationToken.None), Times.Once);
            _mocker.GetMock<ICursoRepository>().Verify(r => r.Atualizar(It.IsAny<Curso>(), CancellationToken.None), Times.Once);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
            Assert.Equal(curso.Nome, command.Nome);
            Assert.Equal(curso.Valor, command.Valor);
            Assert.Equal(curso.ConteudoProgramatico.Descricao, command.DescricaoConteudo);
            Assert.Equal(curso.ConteudoProgramatico.CargaHoraria, command.CargaHoraria);
        }

        [Fact(DisplayName = "Adicionar primeira aula válida")]
        [Trait("Categoria", "Gestao Conteudo - CursoCommandHandler")]
        public async Task AdicionarPrimeiraAula_CommandValido_DeveExecutarComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Conteudo Programatico", 20), 500, true);

            var command = new AdicionarAulaCommand("Aula 1", "Conteudo da aula", 1, "Material", curso.Id);

            _mocker.GetMock<ICursoRepository>().Setup(x => x.ObterComAulasPorId(command.CursoId, CancellationToken.None)).ReturnsAsync(curso);
            _mocker.GetMock<ICursoRepository>().Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.ObterComAulasPorId(command.CursoId, CancellationToken.None), Times.Once);
            _mocker.GetMock<ICursoRepository>().Verify(r => r.InserirAula(It.IsAny<Aula>(), CancellationToken.None), Times.Once);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);

            Assert.Single(curso.Aulas);
        }

        [Fact(DisplayName = "Adicionar segunda aula válida")]
        [Trait("Categoria", "Gestao Conteudo - CursoCommandHandler")]
        public async Task AdicionarSegundaAula_CommandValido_DeveExecutarComSucesso()
        {
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Conteudo Programatico", 20), 500, true);
            var tituloValido = "Estrutura de Dados";
            var conteudoValido = "Conteudo da aula";
            var ordemValida = 1;
            var materialValido = "Material da Aula";
            var aula = new Aula(tituloValido, conteudoValido, ordemValida, materialValido);
            curso.AdicionarAula(aula);

            var command = new AdicionarAulaCommand("Estrutura de Dados II", "Conteudo da aula de Estrutura de Dados II", 2, "Material de Estrutura de Dados II", curso.Id);

            _mocker.GetMock<ICursoRepository>().Setup(x => x.ObterComAulasPorId(command.CursoId, CancellationToken.None)).ReturnsAsync(curso);
            _mocker.GetMock<ICursoRepository>().Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsValid);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.ObterComAulasPorId(command.CursoId, CancellationToken.None), Times.Once);
            _mocker.GetMock<ICursoRepository>().Verify(r => r.InserirAula(It.IsAny<Aula>(), CancellationToken.None), Times.Once);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);

            Assert.Equal(2, curso.Aulas.Count);
        }

        [Fact(DisplayName = "Adicionar aula sem curso existente")]
        [Trait("Categoria", "Gestao Conteudo - CursoCommandHandler")]
        public async Task AdicionarAula_DeveRetornarFalso_QuandoCursoNaoExiste()
        {
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Conteudo Programatico", 20), 500, true);
            var tituloValido = "Estrutura de Dados";
            var conteudoValido = "Conteudo da aula";
            var ordemValida = 1;
            var materialValido = "Material da Aula";
            var aula = new Aula(tituloValido, conteudoValido, ordemValida, materialValido);
            curso.AdicionarAula(aula);

            var command = new AdicionarAulaCommand(tituloValido, "Conteudo da aula de Estrutura de Dados II", 2, "Material de Estrutura de Dados II", Guid.NewGuid());

            _mocker.GetMock<ICursoRepository>().Setup(x => x.ObterComAulasPorId(Guid.NewGuid(), CancellationToken.None)).ReturnsAsync((Curso?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsValid);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.ObterComAulasPorId(command.CursoId, CancellationToken.None), Times.Once);

            Assert.Single(curso.Aulas);
        }

        [Fact(DisplayName = "Adicionar aula ja cadastrada")]
        [Trait("Categoria", "Gestao Conteudo - CursoCommandHandler")]
        public async Task AdicionarAula_DeveRetornarFalso_QuandoOTituloAulaJaExiste()
        {
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Conteudo Programatico", 20), 500, true);
            var tituloValido = "Estrutura de Dados";
            var conteudoValido = "Conteudo da aula";
            var ordemValida = 1;
            var materialValido = "Material da Aula";
            var aula = new Aula(tituloValido, conteudoValido, ordemValida, materialValido);
            curso.AdicionarAula(aula);

            var command = new AdicionarAulaCommand(tituloValido, "Conteudo da aula de Estrutura de Dados II", 2, "Material de Estrutura de Dados II", curso.Id);

            _mocker.GetMock<ICursoRepository>().Setup(x => x.ObterComAulasPorId(command.CursoId, CancellationToken.None)).ReturnsAsync(curso);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsValid);
            _mocker.GetMock<ICursoRepository>().Verify(x => x.ObterComAulasPorId(command.CursoId, CancellationToken.None), Times.Once);

            Assert.Single(curso.Aulas);
        }
    }
}
