namespace PlataformaEducacao.GestaoAluno.Domain.Services
{
    public interface ICertificadoService
    {
        Task<byte[]> GerarCertificado(Certificado certificado);
    }
}
