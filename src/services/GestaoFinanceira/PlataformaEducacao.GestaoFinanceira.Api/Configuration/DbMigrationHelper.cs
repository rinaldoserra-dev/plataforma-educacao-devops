using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.GestaoFinanceira.Api.Data;
using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Api.Configuration
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

                var financeiroContext = scope.ServiceProvider.GetRequiredService<PagamentosContext>();

                if (env.EnvironmentName == "Development" || env.EnvironmentName == "Testing")
                {
                    await financeiroContext.Database.MigrateAsync();

                    await SeedTablesGestaoFinanceira(financeiroContext);
                }
            }

        }

        private static async Task SeedTablesGestaoFinanceira(PagamentosContext pagamentosContext)
        {
            if (!pagamentosContext.Pagamentos.Any())
            {
                var valor = 500.00m;
                var pagamento = new Pagamento
                {
                    Id = Guid.NewGuid(),
                    AlunoId = Guid.Parse("37e95975-6489-4323-8d2c-72cc91a5e3aa"),
                    MatriculaId = Guid.Parse("c4d59bd9-0743-44ea-8ffe-6ac681c52dd6"),
                    TipoPagamento = TipoPagamento.CartaoCredito,
                    Valor = valor,
                    DadosCartao = new DadosCartao("Fulano de Tal", "4916573380937962", "12/28", "123")

                };

                pagamento.AdicionarTransacao(new Transacao
                {
                    CodigoAutorizacao = "",
                    BandeiraCartao = "MasterCard",
                    DataTransacao = DateTime.UtcNow,
                    ValorTotal = valor,
                    CustoTransacao = valor * (decimal)0.03,
                    Status = StatusTransacao.Pago,
                    TID = GetGenericCode(),
                    NSU = GetGenericCode()
                });


                await pagamentosContext.Pagamentos.AddAsync(pagamento);

                await pagamentosContext.SaveChangesAsync();
            }
        }

        private static string GetGenericCode()
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 10)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
    }
}