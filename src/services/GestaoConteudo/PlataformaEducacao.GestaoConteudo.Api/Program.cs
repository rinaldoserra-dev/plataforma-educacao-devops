using PlataformaEducacao.GestaoConteudo.Api.Configurations;
using PlataformaEducacao.WebApi.Core.Extensions;
using PlataformaEducacao.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppSettings();
builder.Host.AddLoggingConfiguration(builder.Configuration, "GestaoConteudo");
builder.Services.AddCorrelationIdConfiguration(builder.Configuration);

builder.Services
    .AddApiConfiguration(builder.Configuration)
    .AddSwaggerConfiguration()
    .AddDbContextConfig(builder.Configuration, builder.Environment)
    .AddJwtConfiguration(builder.Configuration)
    .RegisterServices()
    .AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();

app.UseLoggingConfiguration()
   .UseSwaggerConfiguration()
   .UseApiConfiguration(app.Environment);

app.UseDbMigrationHelper();

app.Run();

public partial class Program
{
}
