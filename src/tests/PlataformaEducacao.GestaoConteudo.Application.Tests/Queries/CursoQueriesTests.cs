using Moq;
using PlataformaEducacao.GestaoConteudo.Application.Queries;
using PlataformaEducacao.GestaoConteudo.Domain;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;

namespace PlataformaEducacao.GestaoConteudo.Application.Tests.Queries
{
    public class CursoQueriesTests
    {
        private readonly Mock<ICursoRepository> _cursoRepositoryMock;
        private readonly CursoQueries _queries;

        public CursoQueriesTests()
        {
            _cursoRepositoryMock = new Mock<ICursoRepository>();
            _queries = new CursoQueries(_cursoRepositoryMock.Object);
        }

        [Fact(DisplayName = "Deve retornar nullo se nao encontrar curso")]
        [Trait("Categoria", "Gestao Conteudo - CursoQueries")]
        public async Task ObterPorId_DeveRetornarNulo_QuandoNaoEncontrarCurso()
        {
            // Arrange
            var cursoId = Guid.NewGuid();
            _cursoRepositoryMock.Setup(r => r.ObterPorId(cursoId, default)).ReturnsAsync((Curso)null!);

            // Act
            var resultado = await _queries.ObterPorId(cursoId, default);

            // Assert
            Assert.Null(resultado);
        }

        [Fact(DisplayName = "Deve retornar curso quando existir")]
        [Trait("Categoria", "Gestao Conteudo - CursoQueries")]
        public async Task ObterPorId_DeveRetornarCursoCiewModel_QuandoCursoExistir()
        {
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Descricao do conteudo", 200), 500, true);
            _cursoRepositoryMock.Setup(r => r.ObterPorId(curso.Id, default)).ReturnsAsync(curso);

            // Act
            var resultado = await _queries.ObterPorId(curso.Id, default);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(curso.Id, resultado.Id);
            Assert.Equal(curso.Nome, resultado.Nome);
        }

        [Fact(DisplayName = "Deve retornar lista com todos os cursos")]
        [Trait("Categoria", "Gestao Conteudo - CursoQueries")]
        public async Task ObterTodos_DeveRetornarTodosCursos()
        {
            // Arrange
            var cursos = new List<Curso>
            {
                new Curso("Curso C#", new ConteudoProgramatico("Descricao do conteudo", 200), 500, true),
                new Curso("Curso Angular", new ConteudoProgramatico("Descricao do conteudo", 150), 450, true)
            };

            _cursoRepositoryMock.Setup(r => r.ObterTodos(default)).ReturnsAsync(cursos);

            // Act
            var resultado = await _queries.ObterTodos(default);

            // Assert
            Assert.Equal(2, resultado.Count());
        }

        [Fact(DisplayName = "Deve retornar lista com todos os cursos disponiveis com as aulas")]
        [Trait("Categoria", "Gestao Conteudo - CursoQueries")]
        public async Task ObterDisponiveisComAula_DeveRetornarTodosCursosDisponiveis()
        {
            // Arrange
            var cursos = new List<Curso>
            {
                new Curso("Curso C#", new ConteudoProgramatico("Descricao do conteudo", 200), 500, true),
                new Curso("Curso Angular", new ConteudoProgramatico("Descricao do conteudo", 150), 450, false)
            };

            _cursoRepositoryMock.Setup(r => r.ObterDisponiveisComAula(default)).ReturnsAsync(cursos.Where(c => c.Disponivel));

            // Act
            var resultado = await _queries.ObterDisponiveisComAula(default);

            // Assert
            Assert.Single(resultado);
        }

        [Fact(DisplayName = "Deve retornar lista com todos as aulas do curso")]
        [Trait("Categoria", "Gestao Conteudo - CursoQueries")]
        public async Task ObterAulasPorCursoId_DeveRetornarAulas()
        {
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Descricao do conteudo", 200), 500, true);
            curso.AdicionarAula(new Aula("Aula 1", "Conteudo da Aula 1", 1, "Link do material 1"));
            curso.AdicionarAula(new Aula("Aula 2", "Conteudo da Aula 2", 2, "Link do material 2"));

            _cursoRepositoryMock.Setup(r => r.ObterComAulasPorId(curso.Id, default)).ReturnsAsync(curso);

            // Act
            var resultado = await _queries.ObterAulasPorCursoId(curso.Id, default);

            // Assert
            Assert.Equal(curso.Aulas.Count(), resultado.Count());
        }
        [Fact(DisplayName = "Deve retornar o curso com as aulas do curso")]
        [Trait("Categoria", "Gestao Conteudo - CursoQueries")]
        public async Task ObterCursoComAulasPorCursoId_DeveRetornarCursoComAulas()
        {
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Descricao do conteudo", 200), 500, true);
            curso.AdicionarAula(new Aula("Aula 1", "Conteudo da Aula 1", 1, "Link do material 1"));
            curso.AdicionarAula(new Aula("Aula 2", "Conteudo da Aula 2", 2, "Link do material 2"));

            _cursoRepositoryMock.Setup(r => r.ObterComAulasPorId(curso.Id, default)).ReturnsAsync(curso);

            // Act
            var resultado = await _queries.ObterCursoComAulasPorCursoId(curso.Id, default);

            // Assert
            Assert.Equal(curso.Aulas.Count(), resultado!.Aulas.Count());
        }
        [Fact(DisplayName = "Deve retornar o curso com as aulas do curso")]
        [Trait("Categoria", "Gestao Conteudo - CursoQueries")]
        public async Task ObterCursoComAulasPorCursoId_DeveRetornarNullSeNaoEncontrarCurso()
        {
            var cursoInexistenteId = Guid.NewGuid();
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Descricao do conteudo", 200), 500, true);
            curso.AdicionarAula(new Aula("Aula 1", "Conteudo da Aula 1", 1, "Link do material 1"));
            curso.AdicionarAula(new Aula("Aula 2", "Conteudo da Aula 2", 2, "Link do material 2"));

            _cursoRepositoryMock.Setup(r => r.ObterComAulasPorId(cursoInexistenteId, default)).ReturnsAsync((Curso)null!);

            // Act
            var resultado = await _queries.ObterCursoComAulasPorCursoId(cursoInexistenteId, default);

            // Assert
            Assert.Null(resultado);
        }
        [Fact(DisplayName = "Deve retornar a aula do curso")]
        [Trait("Categoria", "Gestao Conteudo - CursoQueries")]
        public async Task ObterAulaPorCursoIdEAulaId_DeveRetornarAula()
        {
            // Arrange
            var curso = new Curso("Curso C#", new ConteudoProgramatico("Descricao do conteudo", 200), 500, true);
            var aula = new Aula("Aula 1", "Conteudo da Aula 1", 1, "Link do material 1");
            curso.AdicionarAula(aula);

            _cursoRepositoryMock.Setup(r => r.ObterAulaPorCursoIdEAulaId(curso.Id, aula.Id, default)).ReturnsAsync(aula);

            // Act
            var resultado = await _queries.ObterAulaPorCursoIdEAulaId(curso.Id, aula.Id, default);

            // Assert
            Assert.Equal(resultado!.Id, aula.Id);
            Assert.Equal(resultado!.CursoId, aula.CursoId);
        }

    }
}
