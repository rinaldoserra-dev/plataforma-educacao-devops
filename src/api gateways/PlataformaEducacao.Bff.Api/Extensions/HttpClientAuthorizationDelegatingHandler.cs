using PlataformaEducacao.WebApi.Core.Usuario;

namespace PlataformaEducacao.Bff.Api.Extensions
{
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IAspNetUser _aspNetUser;

        public HttpClientAuthorizationDelegatingHandler(IAspNetUser aspNetUser)
        {
            _aspNetUser = aspNetUser;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _aspNetUser.ObterHttpContext().Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", new List<string>() { authorizationHeader! });
            }

            //var token = _aspNetUser.ObterUserToken();

            //if (!string.IsNullOrEmpty(authorizationHeader))
            //{
            //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1MTE0OGJlNy05ZDA5LTQwOWUtYTFiNi0xZDI1ODI4YzM2NjkiLCJlbWFpbCI6ImFkbWluQHRlc3RlLmNvbSIsImp0aSI6IjQ5OGUxMWVjLTM0ZjktNDlkOS1iYzY0LWU3NTk3OGIyMGQ3NCIsIm5iZiI6MTc3MDM5MjI4MywiaWF0IjoxNzcwMzkyMjgzLCJyb2xlIjoiQURNSU4iLCJleHAiOjE3NzAzOTk0ODMsImlzcyI6IlBsYXRhZm9ybWFFZHVjYWNhbyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0In0.N8St5rJDb-iUgsJbc5WhESypBwXMhbAFZ6IUNbsCmRM");
            //}

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
