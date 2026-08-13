using PlataformaEducacao.GestaoFinanceira.Api.Configuration;
using PlataformaEducacao.GestaoFinanceira.Business.Facade;
using PlataformaEducacao.WebApi.Core.Extensions;
using PlataformaEducacao.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppSettings();
builder.Host.AddLoggingConfiguration(builder.Configuration, "GestaoFinanceira");
builder.Services.AddCorrelationIdConfiguration(builder.Configuration);

builder.Services
    .AddApiConfiguration(builder.Configuration)
    .AddSwaggerConfiguration()
    .AddDbContextConfig(builder.Configuration, builder.Environment)
    .AddJwtConfiguration(builder.Configuration)
    .AddMessageBusConfiguration(builder.Configuration)
    .RegisterServices();

builder.Services.Configure<PagamentoConfig>(
    builder.Configuration.GetSection("PagamentoConfig"));

var app = builder.Build();

app.UseLoggingConfiguration()
   .UseSwaggerConfiguration()
   .UseApiConfiguration(app.Environment);

app.UseDbMigrationHelper();

app.Run();

public partial class Program
{
}
