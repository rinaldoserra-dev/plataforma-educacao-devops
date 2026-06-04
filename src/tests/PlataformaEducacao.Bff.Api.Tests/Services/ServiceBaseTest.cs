using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.Core.Communication;
using System.Net;
using System.Text;
using System.Text.Json;

namespace PlataformaEducacao.Bff.Api.Tests.Services
{
    public class ServiceBaseTest
    {
        private readonly TestableService _service = new();

        [Fact(DisplayName = "ObterConteudo deve serializar objeto como JSON UTF8")]
        [Trait("Categoria", "Bff.Api - Services - Service")]
        public async Task ObterConteudo_DeveSerializarComoJson()
        {
            // Arrange
            var dado = new { Nome = "Teste", Valor = 100 };

            // Act
            var conteudo = _service.TestObterConteudo(dado);

            // Assert
            Assert.NotNull(conteudo);
            var json = await conteudo.ReadAsStringAsync();
            Assert.Contains("Teste", json);
            Assert.Contains("100", json);
        }

        [Fact(DisplayName = "DeserializarObjetoResponse com JSON válido deve retornar ResponseResult")]
        [Trait("Categoria", "Bff.Api - Services - Service")]
        public async Task DeserializarObjetoResponse_ComJsonValido_DeveRetornarResponseResult()
        {
            // Arrange
            var responseResult = new ResponseResult { Sucesso = true, Status = 200, Data = null, Erros = new ResponseErrorMessages() };
            var json = JsonSerializer.Serialize(responseResult);
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            // Act
            var result = await _service.TestDeserializarObjetoResponse(httpResponse);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal(200, result.Status);
        }

        [Fact(DisplayName = "DeserializarObjetoResponse com JSON inválido deve retornar fallback")]
        [Trait("Categoria", "Bff.Api - Services - Service")]
        public async Task DeserializarObjetoResponse_ComJsonInvalido_DeveRetornarFallback()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("invalid json {{{", Encoding.UTF8, "application/json"),
                ReasonPhrase = "Internal Server Error"
            };

            // Act
            var result = await _service.TestDeserializarObjetoResponse(httpResponse);

            // Assert
            Assert.False(result.Sucesso);
            Assert.Equal(500, result.Status);
            Assert.Contains("Internal Server Error", result.Erros.Mensagens);
        }

        [Fact(DisplayName = "DeserializarObjetoResponse com conteúdo vazio deve retornar fallback")]
        [Trait("Categoria", "Bff.Api - Services - Service")]
        public async Task DeserializarObjetoResponse_ComConteudoVazio_DeveRetornarFallback()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.NoContent)
            {
                Content = new StringContent("", Encoding.UTF8, "application/json"),
                ReasonPhrase = "No Content"
            };

            // Act
            var result = await _service.TestDeserializarObjetoResponse(httpResponse);

            // Assert
            Assert.Equal(204, result.Status);
        }

        [Fact(DisplayName = "DeserializarObjetoResponse com conteúdo vazio e ReasonPhrase deve usar ReasonPhrase")]
        [Trait("Categoria", "Bff.Api - Services - Service")]
        public async Task DeserializarObjetoResponse_ComReasonPhrase_DeveUsarReasonPhrase()
        {
            // Arrange
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("", Encoding.UTF8, "application/json"),
                ReasonPhrase = "Bad Gateway"
            };

            // Act
            var result = await _service.TestDeserializarObjetoResponse(httpResponse);

            // Assert
            Assert.Equal(502, result.Status);
            Assert.Single(result.Erros.Mensagens);
        }

        [Fact(DisplayName = "DeserializarData com JsonElement deve deserializar corretamente")]
        [Trait("Categoria", "Bff.Api - Services - Service")]
        public void DeserializarData_ComJsonElement_DeveDeserializar()
        {
            // Arrange
            var obj = new { Nome = "Curso", Valor = 500 };
            var json = JsonSerializer.Serialize(obj);
            var element = JsonSerializer.Deserialize<JsonElement>(json);
            var responseResult = new ResponseResult { Data = element };

            // Act
            var result = _service.TestDeserializarData<TestDto>(responseResult);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Curso", result!.Nome);
            Assert.Equal(500, result.Valor);
        }

        [Fact(DisplayName = "DeserializarData com tipo direto deve retornar objeto")]
        [Trait("Categoria", "Bff.Api - Services - Service")]
        public void DeserializarData_ComTipoDireto_DeveRetornarObjeto()
        {
            // Arrange
            var dto = new TestDto { Nome = "Direto", Valor = 999 };
            var responseResult = new ResponseResult { Data = dto };

            // Act
            var result = _service.TestDeserializarData<TestDto>(responseResult);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Direto", result!.Nome);
        }

        [Fact(DisplayName = "DeserializarData com null deve retornar default")]
        [Trait("Categoria", "Bff.Api - Services - Service")]
        public void DeserializarData_ComNull_DeveRetornarDefault()
        {
            // Arrange
            var responseResult = new ResponseResult { Data = null };

            // Act
            var result = _service.TestDeserializarData<TestDto>(responseResult);

            // Assert
            Assert.Null(result);
        }

        public class TestDto
        {
            public string Nome { get; set; } = string.Empty;
            public int Valor { get; set; }
        }

        private class TestableService : Service
        {
            public StringContent TestObterConteudo(object dado) => ObterConteudo(dado);
            public Task<ResponseResult> TestDeserializarObjetoResponse(HttpResponseMessage msg) => DeserializarObjetoResponse(msg);
            public T? TestDeserializarData<T>(ResponseResult resp) => DeserializarData<T>(resp);
        }
    }
}
