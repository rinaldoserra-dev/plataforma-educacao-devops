using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PlataformaEducacao.Bff.Api.Services;
using CoreResponseResult = PlataformaEducacao.Core.Communication.ResponseResult;
using PlataformaEducacao.Bff.Api.Models.Request.Identidade;

namespace PlataformaEducacao.Bff.Api.Tests.Config
{
    public class PlataformaEducacaoBffAppFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AppSettings:Secret"] = "c1f51f42-5727-4d15-b787-c6bbbb645024",
                    ["AppSettings:ExpiracaoHoras"] = "2",
                    ["AppSettings:ExpiracaoRefreshToken"] = "8",
                    ["AppSettings:Emissor"] = "PlataformaEducacao",
                    ["AppSettings:ValidoEm"] = "https://localhost",
                    ["IdentidadeUrl"] = "https://localhost:5431",
                    ["GestaoConteudoUrl"] = "https://localhost:5441",
                    ["GestaoAlunosUrl"] = "https://localhost:5461",
                    ["GestaoFinanceiraUrl"] = "https://localhost:5273",
                    ["MessageQueueConnection:MessageBus"] = "host=localhost:5672;publisherConfirms=true;timeout=10"
                });
            });
            builder.ConfigureServices(services =>
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                services.RemoveAll<IIdentidadeService>();
                services.RemoveAll<ICursosService>();
                services.RemoveAll<IAlunosService>();
                services.RemoveAll<IPagamentoService>();
                services.RemoveAll<IHealthCheckService>();

                services.AddScoped<IIdentidadeService, FakeIdentidadeService>();
                services.AddScoped<ICursosService, FakeCursosService>();
                services.AddScoped<IAlunosService, FakeAlunosService>();
                services.AddScoped<IPagamentoService, FakePagamentoService>();
                services.AddScoped<IHealthCheckService, FakeHealthCheckService>();
            });
        }
    }

    internal class FakeIdentidadeService : IIdentidadeService
    {
        public Task<CoreResponseResult> Login(PlataformaEducacao.Bff.Api.Models.Request.Identidade.LoginRequest login)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = new
                {
                    accessToken = "fake-token",
                    expiresIn = 7200
                }
            });
        }

        public Task<CoreResponseResult> RegistrarAluno(PlataformaEducacao.Bff.Api.Models.Request.Identidade.RegistroAlunoRequest aluno)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status201Created,
                Data = new { aluno.Email }
            });
        }

        public Task<CoreResponseResult> RefreshToken(RefreshTokenRequest refreshToken)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = new
                {
                    RefreshToken = "fake-refresh-token"
                }
            });
        }
    }

    internal class FakeCursosService : ICursosService
    {
        public Task<CoreResponseResult> AdicionarAula(PlataformaEducacao.Bff.Api.Models.Request.GestaoConteudo.AdicionarAulaRequest aulaRequest)
            => Ok(new { aulaRequest.CursoId, aulaRequest.Titulo });

        public Task<CoreResponseResult> AdicionarCurso(PlataformaEducacao.Bff.Api.Models.Request.GestaoConteudo.AdicionarCursoRequest cursoRequest)
            => Ok(new { cursoRequest.Nome });

        public Task<CoreResponseResult> AtualizarCurso(Guid cursoId, PlataformaEducacao.Bff.Api.Models.Request.GestaoConteudo.AtualizarCursoRequest cursoRequest)
            => Ok(new { cursoId, cursoRequest.Nome });

        public Task<CoreResponseResult> ObterCursoComAulasPorCursoId(Guid cursoId)
            => Ok(new
            {
                Id = cursoId,
                Nome = "Curso de Microsservicos",
                Valor = 199.90m,
                Disponivel = true,
                Aulas = new[]
                {
                    new { Id = Guid.NewGuid() },
                    new { Id = Guid.NewGuid() }
                }
            });

        public Task<CoreResponseResult> ObterCursosDisponiveisComAula()
            => Ok(new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    Nome = "Curso de Microsservicos",
                    Valor = 199.90m
                }
            });

        public Task<CoreResponseResult> ObterTodos()
            => ObterCursosDisponiveisComAula();

        private static Task<CoreResponseResult> Ok(object data)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = data
            });
        }
    }

    internal class FakeAlunosService : IAlunosService
    {
        public Task<HttpResponseMessage> BaixarCertificado(Guid certificadoId)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 1, 2, 3 })
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "certificado.pdf"
            };

            return Task.FromResult(response);
        }

        public Task<CoreResponseResult> FinalizarCurso(PlataformaEducacao.Bff.Api.Models.GestaoAlunos.FinalizarCursoDTO finalizarCurso)
            => Ok(new { finalizarCurso.MatriculaId });

        public Task<CoreResponseResult> Matricular(PlataformaEducacao.Bff.Api.Models.GestaoAlunos.MatricularDTO solicitarMatricula)
        {
            if (string.IsNullOrWhiteSpace(solicitarMatricula.NomeCurso))
            {
                solicitarMatricula.NomeCurso = "Curso de Microsservicos";
                solicitarMatricula.TotalAulasCurso = 2;
                solicitarMatricula.Valor = 199.90m;
            }

            return Ok(new
            {
                solicitarMatricula.CursoId,
                solicitarMatricula.NomeCurso,
                solicitarMatricula.TotalAulasCurso,
                solicitarMatricula.Valor
            });
        }

        public Task<CoreResponseResult> ObterHistorico(Guid alunoId)
            => Ok(new { AlunoId = alunoId, CursosConcluidos = 1 });

        public Task<CoreResponseResult> ObterMatriculasAtivas()
            => Ok(new[] { new { MatriculaId = Guid.NewGuid(), Status = "Ativa" } });

        public Task<CoreResponseResult> ObterMatriculasPendentesPagamento()
            => Ok(new[] { new { MatriculaId = Guid.NewGuid(), Status = "PendentePagamento" } });

        public Task<CoreResponseResult> RealizarAula(PlataformaEducacao.Bff.Api.Models.GestaoAlunos.RealizarAulaDTO realizarAula)
            => Ok(new { realizarAula.MatriculaId, realizarAula.AulaId });

        public Task<CoreResponseResult> ValidarCertificado(string codigoVerificacao)
            => Ok(new { Codigo = codigoVerificacao, Valido = true });

        private static Task<CoreResponseResult> Ok(object data)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = data
            });
        }
    }

    internal class FakePagamentoService : IPagamentoService
    {
        public Task<CoreResponseResult> HealthCheck()
            => Ok(new { });

        public Task<CoreResponseResult> ObterStatus(Guid matriculaId)
            => Ok(new { MatriculaId = matriculaId, Status = "Autorizado" });

        public Task<CoreResponseResult> PagarMatricula(PlataformaEducacao.Bff.Api.Models.GestaoFinanceira.PagarMatriculaDTO pagamento)
            => Ok(new { pagamento.MatriculaId, pagamento.Valor });

        private static Task<CoreResponseResult> Ok(object data)
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = data
            });
        }
    }

    internal class FakeHealthCheckService : IHealthCheckService
    {
        public Task<CoreResponseResult> VerificarSaude()
        {
            return Task.FromResult(new CoreResponseResult
            {
                Sucesso = true,
                Status = StatusCodes.Status200OK,
                Data = new
                {
                    Gateway = "PlataformaEducacao.Bff.Api",
                    Dependencias = new[]
                    {
                        new { Servico = "Identidade", Saudavel = true },
                        new { Servico = "Gestao de Conteudo", Saudavel = true },
                        new { Servico = "Gestao de Alunos", Saudavel = true },
                        new { Servico = "Gestao Financeira", Saudavel = true }
                    }
                }
            });
        }
    }
}
