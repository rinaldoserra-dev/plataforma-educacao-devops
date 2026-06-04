namespace PlataformaEducacao.Core.Communication
{
    public class ResponseResult
    {
        public int Status { get; set; }
        public bool Sucesso { get; set; }
        public object? Data { get; set; }
        public ResponseErrorMessages Erros { get; set; } = new();
    }

    public class ResponseErrorMessages
    {
        public ResponseErrorMessages()
        {
            Mensagens = new List<string>();
        }

        public List<string> Mensagens { get; set; }
    }
}
