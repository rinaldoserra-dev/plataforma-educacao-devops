using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Data.Tests.Data
{
    public class MediatorExtensionTests //: IDisposable
    {
        /*        private readonly SqliteConnection _connection;
                private readonly GestaoAlunoContext _context;
                private readonly Mock<IMediatorHandler> _mediatorMock;

                public MediatorExtensionTests()
                {
                    _connection = new SqliteConnection("DataSource=:memory:");
                    _connection.Open();

                    _mediatorMock = new Mock<IMediatorHandler>();
                    _mediatorMock.Setup(m => m.PublishEvent(It.IsAny<Event>())).Returns(Task.CompletedTask);

                    var options = new DbContextOptionsBuilder<GestaoAlunoContext>()
                        .UseSqlite(_connection)
                        .Options;

                    _context = new GestaoAlunoContext(options, _mediatorMock.Object);
                    _context.Database.EnsureCreated();
                }

                [Fact(DisplayName = "PublicarEventos deve publicar eventos de entidades rastreadas")]
                [Trait("Categoria", "Gestão Aluno - Data - MediatorExtension")]
                public async Task PublicarEventos_DevePublicarEventosDeEntidades()
                {
                    // Arrange
                    var aluno = new Aluno(Guid.NewGuid(), "Aluno Evento", "evento@test.com");
                    var matricula = new Matricula(Guid.NewGuid(), "Curso Evt", totalAulasCurso: 1, valor: 50m);
                    aluno.RealizarMatricula(matricula);

                    await _context.Alunos.AddAsync(aluno);
                    await _context.Matriculas.AddAsync(matricula);
                    await _context.SaveChangesAsync();

                    // Ativar gera evento MatriculaAtivadaEvent
                    var tracked = await _context.Matriculas
                        .Include(m => m.Aluno)
                        .FirstAsync(m => m.Id == matricula.Id);
                    tracked.Aluno.ConcluirPagamentoMatricula(tracked);

                    // Act
                    await _mediatorMock.Object.PublicarEventos(_context);

                    // Assert
                    _mediatorMock.Verify(m => m.PublishEvent(It.IsAny<Event>()), Times.AtLeastOnce);
                }

                [Fact(DisplayName = "PublicarEventos sem eventos não deve chamar PublishEvent")]
                [Trait("Categoria", "Gestão Aluno - Data - MediatorExtension")]
                public async Task PublicarEventos_SemEventos_NaoDevePublicar()
                {
                    // Arrange
                    var aluno = new Aluno(Guid.NewGuid(), "Aluno Sem Evento", "semevento@test.com");
                    await _context.Alunos.AddAsync(aluno);
                    await _context.SaveChangesAsync();

                    // limpar qualquer evento anterior
                    _mediatorMock.Invocations.Clear();

                    // Act
                    await _mediatorMock.Object.PublicarEventos(_context);

                    // Assert
                    _mediatorMock.Verify(m => m.PublishEvent(It.IsAny<Event>()), Times.Never);
                }

                public void Dispose()
                {
                    _context.Dispose();
                    _connection.Dispose();
                }*/
    }
}
