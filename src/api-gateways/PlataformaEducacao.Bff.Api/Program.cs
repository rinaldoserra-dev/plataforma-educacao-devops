using PlataformaEducacao.Bff.Api.Configurations;
using PlataformaEducacao.WebApi.Core.Extensions;
using PlataformaEducacao.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppSettings();
builder.Host.AddLoggingConfiguration(builder.Configuration, "BFF");
builder.Services.AddCorrelationIdConfiguration(builder.Configuration);

builder.Services
    .AddApiConfig(builder.Configuration)
    .AddSwaggerConfiguration()
    .AddJwtConfiguration(builder.Configuration)
    .RegisterServices()
    .AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();

app.UseLoggingConfiguration()
   .UseSwaggerConfiguration()
   .UseApiConfiguration(app.Environment);

app.Run();

public partial class Program
{
}
