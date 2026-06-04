namespace PlataformaEducacao.WebApi.Core.Extensions
{
    public static class ResponseResultExtensions
    {
        //public static async Task<ResponseResult<T>> ToResponseResult<T>(this HttpResponseMessage response)
        //{
        //    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return await response.Content.ReadFromJsonAsync<ResponseResult<T>>(options);
        //    }

        //    if (response.StatusCode == HttpStatusCode.BadRequest)
        //    {
        //        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(options);
        //        return new ResponseResult<T>
        //        {
        //            Sucesso = false,
        //            Erros = new ResponseErrorMessages { Mensagens = problem!.Errors.SelectMany(x => x.Value).ToList() }
        //        };
        //    }

        //    return new ResponseResult<T> { Sucesso = false };
        //}
    }
}
