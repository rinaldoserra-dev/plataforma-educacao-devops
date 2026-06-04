using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoIdentidade.Api.Models;
using PlataformaEducacao.GestaoIdentidade.Api.Services;
using PlataformaEducacao.MessageBus;
using PlataformaEducacao.WebApi.Core.Controllers;
using PlataformaEducacao.WebApi.Core.Identidade;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace PlataformaEducacao.GestaoIdentidade.Api.Controllers
{
    [Route("api/identidade")]
    public class IdentidadeController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAutenticacaoService _autenticacaoService;
        private readonly AppSettings _appSettings;

        private readonly IMessageBus _bus;

        public IdentidadeController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IAutenticacaoService autenticacaoService,
                              IOptions<AppSettings> appSettings,
                              IMessageBus bus)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _autenticacaoService = autenticacaoService;
            _appSettings = appSettings.Value;
            _bus = bus;
        }

        [HttpPost("novo-aluno")]
        public async Task<ActionResult> NovoAluno(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = usuarioRegistro.Email,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, usuarioRegistro.Senha);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "ALUNO");

                var alunoResult = await RegistrarAluno(usuarioRegistro);

                if (!alunoResult.ValidationResult.IsValid)
                {
                    await _userManager.DeleteAsync(user);
                    return CustomResponse(alunoResult.ValidationResult);
                }

                return CustomResponse(HttpStatusCode.Created, await GerarJwt(usuarioRegistro.Email));
            }
            foreach (var error in result.Errors)
            {
                AdicionarErroProcessamento(error.Description);
            }

            return CustomResponse();
        }

        [HttpPost("autenticar")]
        public async Task<ActionResult> Autenticar(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha,
                false, true);

            if (result.Succeeded)
            {
                return CustomResponse(HttpStatusCode.OK, await GerarJwt(usuarioLogin.Email));
            }

            if (result.IsLockedOut)
            {
                AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse();
            }

            AdicionarErroProcessamento("Usuário ou Senha incorretos");
            return CustomResponse();
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(UsuarioRefreshToken refreshToken)
        {

            if (refreshToken == null)
            {
                AdicionarErroProcessamento("Refresh Token inválido");
                return CustomResponse();
            }

            if (string.IsNullOrEmpty(refreshToken.RefreshToken))
            {
                AdicionarErroProcessamento("Refresh Token inválido");
                return CustomResponse();
            }

            var token = await _autenticacaoService.ObterRefreshToken(Guid.Parse(refreshToken.RefreshToken));
            if (token == null)
            {
                AdicionarErroProcessamento("Refresh Token expirado");
                return CustomResponse();
            }

            return CustomResponse(HttpStatusCode.OK, await GerarJwt(token.UserName));
        }

        private async Task<UsuarioRespostaLogin> GerarJwt(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var claims = await _userManager.GetClaimsAsync(user!);

            var identityClaims = await ObterClaimsUsuario(claims, user!);
            var encodedToken = CodificarToken(identityClaims);

            var refreshToken = await _autenticacaoService.GerarRefreshToken(user.UserName);

            return ObterRespostaToken(encodedToken, user!, claims, refreshToken);
        }

        private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role", userRole));
            }

            var identityClaims = new ClaimsIdentity();
            identityClaims.AddClaims(claims);

            return identityClaims;
        }

        private string CodificarToken(ClaimsIdentity identityClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _appSettings.Emissor,
                Audience = _appSettings.ValidoEm,
                Subject = identityClaims,
                Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            return tokenHandler.WriteToken(token);
        }

        private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, IdentityUser user, IEnumerable<Claim> claims, RefreshToken refreshToken)
        {
            return new UsuarioRespostaLogin
            {
                AccessToken = encodedToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UsuarioToken = new UsuarioToken
                {
                    Id = user.Id,
                    Email = user.Email!,
                    Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
                }
            };
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

        private async Task<ResponseMessage> RegistrarAluno(UsuarioRegistro usuarioRegistro)
        {
            var usuario = await _userManager.FindByEmailAsync(usuarioRegistro.Email);

            var usuarioRegistrado = new UsuarioRegistradoIntegrationEvent(Guid.Parse(usuario!.Id), usuarioRegistro.Nome, usuarioRegistro.Email);

            try
            {
                return await _bus.RequestAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
            }
            catch
            {
                await _userManager.DeleteAsync(usuario);
                throw;
            }
        }

    }
}
