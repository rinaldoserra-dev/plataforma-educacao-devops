using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoIdentidade.Api.Data;

namespace PlataformaEducacao.GestaoIdentidade.Api.Configurations
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

                var identityContext = scope.ServiceProvider.GetRequiredService<GestaoIdentidadeContext>();

                if (env.EnvironmentName == "Development" || env.EnvironmentName == "Testing" || env.EnvironmentName == "Docker")
                {
                    await identityContext.Database.MigrateAsync();

                    await SeedUserAndRoles(identityContext);
                }
            }

            private static async Task SeedUserAndRoles(GestaoIdentidadeContext contextIdentity)
            {
                if (contextIdentity.Users.Any()) return;

                var roleAdmin = new IdentityRole
                {
                    Name = "ADMIN",
                    NormalizedName = "ADMIN"
                };
                var roleAluno = new IdentityRole
                {
                    Name = "ALUNO",
                    NormalizedName = "ALUNO"
                };

                await contextIdentity.Roles.AddAsync(roleAdmin);
                await contextIdentity.Roles.AddAsync(roleAluno);

                var idAdmin = Guid.NewGuid();
                var usuarioAdmin = new IdentityUser
                {
                    Id = idAdmin.ToString(),
                    Email = "admin@teste.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TESTE.COM",
                    UserName = "admin@teste.com",
                    AccessFailedCount = 0,
                    PasswordHash = "AQAAAAIAAYagAAAAEF/nmfwFGPa8pnY9AvZL8HKI7r7l+aM4nryRB+Y3Ktgo6d5/0d25U2mhixnO4h/K5w==",
                    NormalizedUserName = "ADMIN@TESTE.COM"
                };
                await contextIdentity.Users.AddAsync(usuarioAdmin);

                await contextIdentity.UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    RoleId = roleAdmin.Id,
                    UserId = usuarioAdmin.Id
                });

                var usuarioAluno = new IdentityUser
                {
                    Id = "37e95975-6489-4323-8d2c-72cc91a5e3aa",
                    Email = "aluno@teste.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ALUNO@TESTE.COM",
                    UserName = "aluno@teste.com",
                    AccessFailedCount = 0,
                    PasswordHash = "AQAAAAIAAYagAAAAEF/nmfwFGPa8pnY9AvZL8HKI7r7l+aM4nryRB+Y3Ktgo6d5/0d25U2mhixnO4h/K5w==",
                    NormalizedUserName = "ALUNO@TESTE.COM"
                };
                await contextIdentity.Users.AddAsync(usuarioAluno);
                await contextIdentity.UserRoles.AddAsync(new IdentityUserRole<string>
                {
                    RoleId = roleAluno.Id,
                    UserId = usuarioAluno.Id
                });

                await contextIdentity.SaveChangesAsync();
            }
        }
    }
}
