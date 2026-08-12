using PlataformaEducacao.GestaoAluno.Api.Configurations;
using PlataformaEducacao.WebApi.Core.Extensions;
using PlataformaEducacao.WebApi.Core.Identidade;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configura a licença de QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

builder.Host.ConfigureAppSettings();
builder.Host.AddLoggingConfiguration(builder.Configuration, "GestaoAluno");
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
