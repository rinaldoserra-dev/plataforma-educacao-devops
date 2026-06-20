using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoConteudo.Data;
using PlataformaEducacao.GestaoConteudo.Domain;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;

namespace PlataformaEducacao.GestaoConteudo.Api.Configurations
{
    public static class DbMigrationHelperExtension
    {
        public static void UseDbMigrationHelper(this WebApplication app)
        {
            DbMigrationHelper.EnsureSeedData(app).Wait();
        }

        public static class DbMigrationHelper
        {
            public static async Task EnsureSeedData(WebApplication application)
            {
                var service = application.Services.CreateScope().ServiceProvider;
                await EnsureSeedData(service);
            }
            public static async Task EnsureSeedData(IServiceProvider serviceProvider)
            {
                using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                var conteudoContext = scope.ServiceProvider.GetRequiredService<GestaoConteudoContext>();

                if (env.EnvironmentName is "Development" or "Docker" or "Testing")
                {
                    await conteudoContext.Database.MigrateAsync();

                    await SeedTablesGestaoConteudo(conteudoContext);
                }
            }

            private static async Task SeedTablesGestaoConteudo(GestaoConteudoContext conteudoContext)
            {
                if (!conteudoContext.Cursos.Any())
                {
                    var curso = new Curso(".NET", new ConteudoProgramatico("Conteudo do Curso", 30), 500, true);
                    curso.DefinirId(Guid.Parse("683C31AE-7DB1-4A89-B011-13076E794824"));

                    var aula1 = new Aula($"Aula 1", $"Conteudo da Aula 1", 1, $"Segue link dos materiais da aula 1");
                    aula1.DefinirId(Guid.Parse("C862D7FB-341D-476F-A978-98CBD44C2D08"));
                    curso.AdicionarAula(aula1);

                    var aula2 = new Aula($"Aula 2", $"Conteudo da Aula 2", 2, $"Segue link dos materiais da aula 2");
                    aula2.DefinirId(Guid.Parse("0E7EF4C0-4EB3-4C05-B61F-968D114C21DC"));
                    curso.AdicionarAula(aula2);

                    var aula3 = new Aula($"Aula 3", $"Conteudo da Aula 3", 3, $"Segue link dos materiais da aula 3");
                    aula3.DefinirId(Guid.Parse("40EBD28E-A138-4B53-87F3-20BDA1E71121"));
                    curso.AdicionarAula(aula3);

                    var aula4 = new Aula($"Aula 4", $"Conteudo da Aula 4", 4, $"Segue link dos materiais da aula 4");
                    aula4.DefinirId(Guid.Parse("66CD0D35-A69E-4EFB-AB85-FDE431408DF1"));
                    curso.AdicionarAula(aula4);

                    var aula5 = new Aula($"Aula 5", $"Conteudo da Aula 5", 5, $"Segue link dos materiais da aula 5");
                    aula5.DefinirId(Guid.Parse("2CA8A9DC-CB4D-460F-844C-F80E8AA450A3"));
                    curso.AdicionarAula(aula5);

                    await conteudoContext.Cursos.AddAsync(curso);

                    curso = new Curso(".NET Core", new ConteudoProgramatico("Conteudo do Curso de .NET Core", 30), 500, true);
                    curso.DefinirId(Guid.Parse("2194EB04-6C17-4379-8F07-C847C899466F"));
                    for (int i = 1; i <= 1; i++)
                    {
                        var aula = new Aula($"Aula {i}", $"Conteudo da Aula {i}", i, $"Segue link dos materiais da aula {i}");
                        curso.AdicionarAula(aula);
                    }
                    await conteudoContext.Cursos.AddAsync(curso);

                    curso = new Curso("Dominios Ricos", new ConteudoProgramatico("Conteudo do Curso de Dominios Ricos", 30), 500, true);
                    curso.DefinirId(Guid.Parse("12E04CBC-6ACF-4582-9345-C74B12C8183C"));
                    for (int i = 1; i <= 1; i++)
                    {
                        var aula = new Aula($"Aula {i}", $"Conteudo da Aula {i}", i, $"Segue link dos materiais da aula {i}");
                        aula.DefinirId(Guid.Parse("AB514ABA-7F82-4B3C-AB3E-3B311A516DE9"));
                        curso.AdicionarAula(aula);
                    }
                    await conteudoContext.Cursos.AddAsync(curso);

                    await conteudoContext.SaveChangesAsync();
                }
            }
        }
    }
}
