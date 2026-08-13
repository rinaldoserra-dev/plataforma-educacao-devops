using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoConteudo.Data.Repository;
using PlataformaEducacao.GestaoConteudo.Domain;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;
using Xunit;

namespace PlataformaEducacao.GestaoConteudo.Data.Tests.Repository
{
    public class CursoRepositoryTests
    {
        [Fact(DisplayName = "ObterAulaPorCursoIdEAulaId deve retornar a aula quando existir")]
        public async Task ObterAulaPorCursoIdEAulaId_DeveRetornarAula_QuandoExistir()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);

            var curso = new Curso("Curso Teste", new ConteudoProgramatico("Conteudo", 10), 100m, true);
            var aula = new Aula("Aula 1", "Conteudo da aula", 1, "Material");
            curso.AdicionarAula(aula);

            await context.Cursos.AddAsync(curso);
            await context.SaveChangesAsync();

            var repository = new CursoRepository(context);

            var resultado = await repository.ObterAulaPorCursoIdEAulaId(curso.Id, aula.Id, default);

            Assert.NotNull(resultado);
            Assert.Equal(aula.Id, resultado.Id);
            Assert.Equal(curso.Id, resultado.CursoId);
        }

        [Fact(DisplayName = "ObterAulaPorCursoIdEAulaId deve retornar null quando nao encontrar a aula")]
        public async Task ObterAulaPorCursoIdEAulaId_DeveRetornarNull_QuandoNaoExistir()
        {
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);

            var curso = new Curso("Curso Teste", new ConteudoProgramatico("Conteudo", 10), 100m, true);
            await context.Cursos.AddAsync(curso);
            await context.SaveChangesAsync();

            var repository = new CursoRepository(context);

            var resultado = await repository.ObterAulaPorCursoIdEAulaId(curso.Id, Guid.NewGuid(), default);

            Assert.Null(resultado);
        }

        private static GestaoConteudoContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<GestaoConteudoContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new GestaoConteudoContext(options);
        }
    }
}
