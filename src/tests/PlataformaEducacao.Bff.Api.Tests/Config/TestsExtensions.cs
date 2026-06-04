using System.Net.Http.Headers;

namespace PlataformaEducacao.Bff.Api.Tests.Config
{
    public static class TestsExtensions
    {
        public static void AtribuirJsonMediaType(this HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static void AutenticarComo(this HttpClient client, string role, Guid? userId = null)
        {
            client.AtribuirJsonMediaType();
            client.DefaultRequestHeaders.Add("X-Test-Role", role);

            if (userId.HasValue)
            {
                client.DefaultRequestHeaders.Add("X-Test-UserId", userId.Value.ToString());
            }
        }
    }
}
