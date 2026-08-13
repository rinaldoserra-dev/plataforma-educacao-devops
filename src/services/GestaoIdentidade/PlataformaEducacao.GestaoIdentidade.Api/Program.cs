using PlataformaEducacao.GestaoIdentidade.Api.Configurations;
using PlataformaEducacao.WebApi.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppSettings();
builder.Host.AddLoggingConfiguration(builder.Configuration, "GestaoIdentidade");
builder.Services.AddCorrelationIdConfiguration(builder.Configuration);

builder.Services
    .AddApiConfig(builder.Configuration)
    .AddSwaggerConfiguration()
    .AddDbContextConfig(builder.Configuration, builder.Environment)
    .AddIdentityConfig(builder.Configuration)
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
