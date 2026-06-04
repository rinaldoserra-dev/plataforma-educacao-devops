using System.Net;
using System.Text;
using System.Text.Json;

namespace PlataformaEducacao.Bff.Api.Tests.Services
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, (HttpStatusCode StatusCode, string Json)> _responses = new();

        public void SetupResponse(string url, HttpStatusCode statusCode, object? content = null)
        {
            var json = content != null
                ? JsonSerializer.Serialize(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : "";
            _responses[url] = (statusCode, json);
        }

        public void SetupResponse(HttpStatusCode statusCode, object? content = null)
        {
            var json = content != null
                ? JsonSerializer.Serialize(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : "";
            _responses["*"] = (statusCode, json);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var url = request.RequestUri?.ToString() ?? "";
            var path = request.RequestUri?.PathAndQuery ?? "";

            foreach (var kvp in _responses)
            {
                if (kvp.Key == "*" || url.Contains(kvp.Key) || path.Contains(kvp.Key))
                {
                    return Task.FromResult(new HttpResponseMessage(kvp.Value.StatusCode)
                    {
                        Content = new StringContent(kvp.Value.Json, Encoding.UTF8, "application/json")
                    });
                }
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }
    }
}
