using QuestPDF.Infrastructure;
using PlataformaEducacao.GestaoAluno.Api.Configurations;
using PlataformaEducacao.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);

// Configura a licença de QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

builder.Host.ConfigureAppSettings();

builder.Services
    .AddApiConfiguration()
    .AddSwaggerConfiguration()
    .AddDbContextConfig(builder.Configuration, builder.Environment)
    .AddJwtConfiguration(builder.Configuration)
    .RegisterServices()
    .AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();

app.UseSwaggerConfiguration()
   .UseApiConfiguration(app.Environment);

app.UseDbMigrationHelper();

app.Run();

public partial class Program { }
