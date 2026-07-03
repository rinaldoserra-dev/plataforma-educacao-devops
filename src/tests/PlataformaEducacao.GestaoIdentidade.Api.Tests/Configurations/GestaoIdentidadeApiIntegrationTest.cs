using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PlataformaEducacao.GestaoIdentidade.Api.Data;
using PlataformaEducacao.GestaoIdentidade.Api.Tests.Config;
using PlataformaEducacao.MessageBus;
using System.Net;
using System.Net.Http.Json;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Configurations
{
    public class GestaoIdentidadeApiIntegrationTest : IClassFixture<GestaoIdentidadeApiFactory>
    {
        private readonly GestaoIdentidadeApiFactory _factory;
        private readonly HttpClient _client;

        public GestaoIdentidadeApiIntegrationTest(GestaoIdentidadeApiFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact(DisplayName = "Swagger endpoint deve retornar OK")]
        [Trait("Categoria", "GestaoIdentidade.Api - Configurations - SwaggerConfig")]
        public async Task SwaggerEndpoint_DeveRetornarOk()
        {
            var response = await _client.GetAsync("/swagger/v1/swagger.json");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Identidade", content);
        }

        [Fact(DisplayName = "Swagger UI deve retornar OK")]
        [Trait("Categoria", "GestaoIdentidade.Api - Configurations - SwaggerConfig")]
        public async Task SwaggerUI_DeveRetornarOk()
        {
            var response = await _client.GetAsync("/swagger/index.html");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DbContext deve estar registrado")]
        [Trait("Categoria", "GestaoIdentidade.Api - Configurations - DbContextConfig")]
        public void DbContext_DeveEstarRegistrado()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetService<GestaoIdentidadeContext>();

            Assert.NotNull(context);
        }

        [Fact(DisplayName = "IMessageBus deve estar registrado")]
        [Trait("Categoria", "GestaoIdentidade.Api - Configurations - MessageBusConfig")]
        public void IMessageBus_DeveEstarRegistrado()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IMessageBus>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "UserManager deve estar registrado")]
        [Trait("Categoria", "GestaoIdentidade.Api - Configurations - IdentityConfig")]
        public void UserManager_DeveEstarRegistrado()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "SignInManager deve estar registrado")]
        [Trait("Categoria", "GestaoIdentidade.Api - Configurations - IdentityConfig")]
        public void SignInManager_DeveEstarRegistrado()
        {
            using var scope = _factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<SignInManager<IdentityUser>>();

            Assert.NotNull(service);
        }

        [Fact(DisplayName = "Rota inexistente deve retornar NotFound")]
        [Trait("Categoria", "GestaoIdentidade.Api - Configurations - ApiConfig")]
        public async Task RotaInexistente_DeveRetornarNotFound()
        {
            var response = await _client.GetAsync("/api/rota-inexistente-xyz");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "Autenticar com dados inválidos deve retornar BadRequest")]
        [Trait("Categoria", "GestaoIdentidade.Api - Controllers - IdentidadeController")]
        public async Task Autenticar_DadosInvalidos_DeveRetornarBadRequest()
        {
            var response = await _client.PostAsJsonAsync("/api/identidade/autenticar", new
            {
                Email = "",
                Senha = ""
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "Autenticar com credenciais incorretas deve retornar BadRequest")]
        [Trait("Categoria", "GestaoIdentidade.Api - Controllers - IdentidadeController")]
        public async Task Autenticar_CredenciaisIncorretas_DeveRetornarBadRequest()
        {
            var response = await _client.PostAsJsonAsync("/api/identidade/autenticar", new
            {
                Email = "naoexiste@teste.com",
                Senha = "Senha@123456"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "NovoAluno com dados inválidos deve retornar BadRequest")]
        [Trait("Categoria", "GestaoIdentidade.Api - Controllers - IdentidadeController")]
        public async Task NovoAluno_DadosInvalidos_DeveRetornarBadRequest()
        {
            var response = await _client.PostAsJsonAsync("/api/identidade/novo-aluno", new
            {
                Nome = "",
                Email = "invalido",
                Senha = "",
                SenhaConfirmacao = ""
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "NovoAluno com dados válidos deve registrar e retornar Created")]
        [Trait("Categoria", "GestaoIdentidade.Api - Controllers - IdentidadeController")]
        public async Task NovoAluno_DadosValidos_DeveRetornarCreated()
        {
            await SeedRoles();
            var email = $"aluno_{Guid.NewGuid():N}@teste.com";

            var response = await _client.PostAsJsonAsync("/api/identidade/novo-aluno", new
            {
                Nome = "Aluno Teste",
                Email = email,
                Senha = "Teste@123456",
                SenhaConfirmacao = "Teste@123456"
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("accessToken", content);
        }

        [Fact(DisplayName = "NovoAluno com email duplicado deve retornar BadRequest")]
        [Trait("Categoria", "GestaoIdentidade.Api - Controllers - IdentidadeController")]
        public async Task NovoAluno_EmailDuplicado_DeveRetornarBadRequest()
        {
            await SeedRoles();
            var email = $"dup_{Guid.NewGuid():N}@teste.com";

            // primeiro registro
            await _client.PostAsJsonAsync("/api/identidade/novo-aluno", new
            {
                Nome = "Aluno",
                Email = email,
                Senha = "Teste@123456",
                SenhaConfirmacao = "Teste@123456"
            });

            // segundo com mesmo email
            var response = await _client.PostAsJsonAsync("/api/identidade/novo-aluno", new
            {
                Nome = "Aluno Dup",
                Email = email,
                Senha = "Teste@123456",
                SenhaConfirmacao = "Teste@123456"
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "Autenticar com credenciais corretas deve retornar OK com token")]
        [Trait("Categoria", "GestaoIdentidade.Api - Controllers - IdentidadeController")]
        public async Task Autenticar_CredenciaisCorretas_DeveRetornarOkComToken()
        {
            await SeedRoles();
            var email = $"auth_{Guid.NewGuid():N}@teste.com";

            // registrar
            await _client.PostAsJsonAsync("/api/identidade/novo-aluno", new
            {
                Nome = "Aluno Auth",
                Email = email,
                Senha = "Teste@123456",
                SenhaConfirmacao = "Teste@123456"
            });

            // autenticar
            var response = await _client.PostAsJsonAsync("/api/identidade/autenticar", new
            {
                Email = email,
                Senha = "Teste@123456"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("accessToken", content);
        }

        private async Task SeedRoles()
        {
            using var scope = _factory.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("ALUNO"))
                await roleManager.CreateAsync(new IdentityRole("ALUNO"));

            if (!await roleManager.RoleExistsAsync("ADMIN"))
                await roleManager.CreateAsync(new IdentityRole("ADMIN"));
        }
    }
}
