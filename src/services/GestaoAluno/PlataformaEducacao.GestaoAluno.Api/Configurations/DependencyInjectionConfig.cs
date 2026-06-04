using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Mediator;
using PlataformaEducacao.GestaoAluno.Application.Commands.AdicionarAluno;
using PlataformaEducacao.GestaoAluno.Application.Commands.FinalizarCurso;
using PlataformaEducacao.GestaoAluno.Application.Commands.GerarCertificado;
using PlataformaEducacao.GestaoAluno.Application.Commands.MatricularAlunoCurso;
using PlataformaEducacao.GestaoAluno.Application.Commands.RealizarAula;
using PlataformaEducacao.GestaoAluno.Application.Events;
using PlataformaEducacao.GestaoAluno.Application.Queries;
using PlataformaEducacao.GestaoAluno.Data.Repository;
using PlataformaEducacao.GestaoAluno.Data.Services;
using PlataformaEducacao.GestaoAluno.Domain.Events;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;
using PlataformaEducacao.GestaoAluno.Domain.Services;
using PlataformaEducacao.WebApi.Core.Usuario;

namespace PlataformaEducacao.GestaoAluno.Api.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {

            // Mediator
            services.AddMediatR(
               typeof(AdicionarAlunoCommand).Assembly,
               typeof(AdicionarAlunoCommandHandler).Assembly
           );

            // API
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddScoped<IMediatorHandler, MediatorHandler>();

            // Commands
            services.AddScoped<IRequestHandler<AdicionarAlunoCommand, ValidationResult>, AdicionarAlunoCommandHandler>();
            services.AddScoped<IRequestHandler<FinalizarCursoCommand, ValidationResult>, FinalizarCursoCommandHandler>();
            services.AddScoped<IRequestHandler<GerarCertificadoCommand, ValidationResult>, GerarCertificadoCommandHandler>();
            services.AddScoped<IRequestHandler<MatricularAlunoCursoCommand, ValidationResult>, MatricularAlunoCursoCommandHandler>();

            services.AddScoped<IRequestHandler<RealizarAulaCommand, ValidationResult>, RealizarAulaCommandHandler>();

            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IAlunoRepository, AlunoRepository>();
            services.AddScoped<IAlunoQueries, AlunoQueries>();
            services.AddScoped<ICertificadoService, CertificadoService>();

            services.AddScoped<INotificationHandler<CursoFinalizadoEvent>, MatriculaEventHandler>();
            services.AddScoped<INotificationHandler<MatriculaAtivadaEvent>, MatriculaEventHandler>();

            return services;
        }
    }
}