using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.GestaoConteudo.Application.Commands;
using PlataformaEducacao.GestaoConteudo.Application.Queries;
using PlataformaEducacao.GestaoConteudo.Data.Repository;
using PlataformaEducacao.GestaoConteudo.Domain;
using PlataformaEducacao.WebApi.Core.Usuario;

namespace PlataformaEducacao.GestaoConteudo.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {

            // Mediator
            services.AddMediatR(
               typeof(AdicionarAulaCommand).Assembly,
               typeof(CursoCommandHandler).Assembly
           );

            // API
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Commands
            services.AddScoped<IRequestHandler<AdicionarAulaCommand, ValidationResult>, CursoCommandHandler>();
            services.AddScoped<IRequestHandler<AdicionarCursoCommand, ValidationResult>, CursoCommandHandler>();
            services.AddScoped<IRequestHandler<AtualizarCursoCommand, ValidationResult>, CursoCommandHandler>();
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<ICursoRepository, CursoRepository>();
            services.AddScoped<ICursoQueries, CursoQueries>();

            return services;
        }
    }
}
