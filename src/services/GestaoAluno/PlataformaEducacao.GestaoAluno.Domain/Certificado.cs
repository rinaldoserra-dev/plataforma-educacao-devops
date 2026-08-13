using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain
{
    public class Certificado : Entity
    {
        public Guid MatriculaId { get; private set; }

        public string CodigoVerificacao { get; private set; } = null!;

        public Matricula Matricula { get; private set; } = null!;

        public Certificado(Guid matriculaId)
        {
            MatriculaId = matriculaId;
            GerarCodigoValidacao();
            Validar();
        }

        protected Certificado()
        {
        }

        protected void Validar()
        {
            Validacoes.ValidarSeVazio(MatriculaId, "A matricula é obrigatória.");
            Validacoes.ValidarSeVazio(CodigoVerificacao, "O código de verificação é obrigatório.");
        }

        private void GerarCodigoValidacao()
        {
            CodigoVerificacao = Guid.NewGuid().ToString();
        }

        public static class CertificadoFactory
        {
            public static Certificado CriarCompleto(Matricula matricula, string codigoVerificacao)
            {
                var certificado = new Certificado
                {
                    MatriculaId = matricula.Id,
                    Matricula = matricula,
                    CodigoVerificacao = codigoVerificacao
                };

                certificado.Validar();
                return certificado;
            }
        }
    }
}
