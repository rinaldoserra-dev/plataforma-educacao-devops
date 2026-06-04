using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoAluno.Data;
using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Api.Configurations
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

                var alunoContext = scope.ServiceProvider.GetRequiredService<GestaoAlunoContext>();

                if (env.EnvironmentName == "Development" || env.EnvironmentName == "Testing")
                {
                    await alunoContext.Database.MigrateAsync();

                    await SeedTablesGestaoAluno(alunoContext);
                }
            }
            private static async Task SeedTablesGestaoAluno(GestaoAlunoContext alunoContext)
            {
                if (!alunoContext.Matriculas.Any())
                {
                    Guid alunoUm = Guid.Parse("37e95975-6489-4323-8d2c-72cc91a5e3aa");
                    var aluno = new Aluno(alunoUm, "Aluno ", "aluno@teste.com");

                    Guid cursoId = Guid.Parse("683C31AE-7DB1-4A89-B011-13076E794824");
                    var cursoNome = ".NET";
                    var matricula = new Matricula(cursoId, cursoNome, 5, 500);

                    Guid cursoCoreId = Guid.Parse("2194EB04-6C17-4379-8F07-C847C899466F");
                    var cursoCoreNome = ".NET Core";
                    var matriculaCore = new Matricula(cursoCoreId, cursoCoreNome, 1, 500);

                    Guid cursoDominiosRicosId = Guid.Parse("12E04CBC-6ACF-4582-9345-C74B12C8183C");
                    var cursoDominiosRicosNome = "Dominios Ricos";
                    var matriculaDominiosRicos = new Matricula(cursoDominiosRicosId, cursoDominiosRicosNome, 1, 500);

                    aluno.RealizarMatricula(matricula);
                    aluno.RealizarMatricula(matriculaCore);
                    aluno.RealizarMatricula(matriculaDominiosRicos);

                    matricula.Ativar();
                    // matriculaCore.Ativar(); // deixada pendente de pagamento, para já aparecer na listagem de pendentes
                    matriculaDominiosRicos.Ativar();


                    var progresso = new ProgressoAula(Guid.Parse("C862D7FB-341D-476F-A978-98CBD44C2D08"));
                    matricula.RegistrarAula(progresso);
                    progresso = new ProgressoAula(Guid.Parse("0E7EF4C0-4EB3-4C05-B61F-968D114C21DC"));
                    matricula.RegistrarAula(progresso);
                    progresso = new ProgressoAula(Guid.Parse("40EBD28E-A138-4B53-87F3-20BDA1E71121"));
                    matricula.RegistrarAula(progresso);
                    progresso = new ProgressoAula(Guid.Parse("66CD0D35-A69E-4EFB-AB85-FDE431408DF1"));
                    matricula.RegistrarAula(progresso);
                    progresso = new ProgressoAula(Guid.Parse("2CA8A9DC-CB4D-460F-844C-F80E8AA450A3"));
                    matricula.RegistrarAula(progresso);
                    matricula.FinalizarCurso();

                    progresso = new ProgressoAula(Guid.Parse("AB514ABA-7F82-4B3C-AB3E-3B311A516DE9"));
                    matriculaDominiosRicos.RegistrarAula(progresso);

                    await alunoContext.Alunos.AddAsync(aluno);

                    var certificado = new Certificado(matricula.Id);
                    await alunoContext.Certificados.AddAsync(certificado);

                    await alunoContext.SaveChangesAsync();
                }
            }
        }
    }
}