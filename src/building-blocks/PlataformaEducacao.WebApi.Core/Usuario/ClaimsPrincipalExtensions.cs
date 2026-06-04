using System.Security.Claims;

namespace PlataformaEducacao.WebApi.Core.Usuario
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            ArgumentNullException.ThrowIfNull(principal);

            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value ?? string.Empty;
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            ArgumentNullException.ThrowIfNull(principal);

            var claim = principal.FindFirst("email");
            return claim?.Value ?? string.Empty;
        }

        public static string GetUserToken(this ClaimsPrincipal principal)
        {
            ArgumentNullException.ThrowIfNull(principal);

            var claim = principal.FindFirst("JWT");
            return claim?.Value ?? string.Empty;
        }

        public static string GetUserRefreshToken(this ClaimsPrincipal principal)
        {
            ArgumentNullException.ThrowIfNull(principal);

            var claim = principal.FindFirst("RefreshToken");
            return claim?.Value ?? string.Empty;
        }
    }
}
