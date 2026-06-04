using PlataformaEducacao.GestaoIdentidade.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppSettings();

builder.Services
    .AddApiConfig()
    .AddSwaggerConfiguration()
    .AddDbContextConfig(builder.Configuration, builder.Environment)
    .AddIdentityConfig(builder.Configuration)
    .RegisterServices()
    .AddMessageBusConfiguration(builder.Configuration);

var app = builder.Build();

app.UseSwaggerConfiguration()
   .UseApiConfiguration(app.Environment);

app.UseDbMigrationHelper();

app.Run();

public partial class Program { }
