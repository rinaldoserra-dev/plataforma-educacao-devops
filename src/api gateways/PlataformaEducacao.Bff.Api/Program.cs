using PlataformaEducacao.Bff.Api.Configurations;
using PlataformaEducacao.WebApi.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppSettings();


builder.Services
    .AddApiConfig(builder.Configuration)
    .AddSwaggerConfiguration()
    .AddJwtConfiguration(builder.Configuration)
    .RegisterServices()
    .AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();

app.UseSwaggerConfiguration()
   .UseApiConfiguration(app.Environment);

app.Run();

public partial class Program { }
