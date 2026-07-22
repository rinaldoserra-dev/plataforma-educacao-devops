using PlataformaEducacao.Core.Communication;
using System.Text;
using System.Text.Json;

namespace PlataformaEducacao.Bff.Api.Services
{
    public abstract class Service
    {
        protected static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        protected StringContent ObterConteudo(object dado)
        {
            return new StringContent(
                JsonSerializer.Serialize(dado, JsonOptions),
                Encoding.UTF8,
                "application/json");
        }
        protected async Task<ResponseResult> DeserializarObjetoResponse(HttpResponseMessage responseMessage)
        {
            var stringContent = await responseMessage.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(stringContent))
            {
                try
                {
                    var result = JsonSerializer.Deserialize<ResponseResult>(stringContent, JsonOptions);
                    if (result != null)
                    {
                        result.Status = (int)responseMessage.StatusCode;
                        return result;
                    }
                }
                catch (JsonException) { }
            }

            return new ResponseResult
            {
                Status = (int)responseMessage.StatusCode,
                Sucesso = responseMessage.IsSuccessStatusCode,
                Erros = new ResponseErrorMessages
                {
                    Mensagens = new List<string> { responseMessage.ReasonPhrase ?? "Erro na comunicação com a API." }
                }
            };
        }

        protected T? DeserializarData<T>(ResponseResult responseResult)
        {
            if (responseResult.Data is JsonElement jsonElement)
            {
                return jsonElement.Deserialize<T>(JsonOptions);
            }

            if (responseResult.Data is T typed)
            {
                return typed;
            }

            return default;
        }

        //protected async Task<T> DeserializarObjetoResponse<T>(HttpResponseMessage responseMessage)
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };

        //    return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
        //}

        //protected bool TratarErrosResponse(HttpResponseMessage response)
        //{
        //    if (response.StatusCode == HttpStatusCode.BadRequest) return false;

        //    response.EnsureSuccessStatusCode();
        //    return true;
        //}
    }
}
