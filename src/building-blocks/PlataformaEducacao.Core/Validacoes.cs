using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.Core
{
    public class Validacoes
    {
        public static void ValidarSeVazio(string? valor, string mensagem)
        {
            if (String.IsNullOrWhiteSpace(valor))
            {
                throw new DomainException(mensagem);
            }
        }

        public static void ValidarSeMenorOuIgualQue(decimal valor, decimal minimo, string mensagem)
        {
            if (valor <= minimo)
            {
                throw new DomainException(mensagem);
            }
        }

        public static void ValidarSeMenorOuIgualQue(int valor, int minimo, string mensagem)
        {
            if (valor <= minimo)
            {
                throw new DomainException(mensagem);
            }
        }

        public static void ValidarSeVazio(Guid? guid, string mensagem)
        {
            if (guid is null || guid == Guid.Empty)
            {
                throw new DomainException(mensagem);
            }
        }
    }
}
