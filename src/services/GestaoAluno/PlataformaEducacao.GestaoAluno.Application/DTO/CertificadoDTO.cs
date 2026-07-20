using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.DTO
{
    public class CertificadoDTO(Guid certificadoId, string nomeAluno, string nomeCurso, DateTime dataConclusao,
        string codigoVerificacao)
    {
        public Guid CertificadoId { get; set; } = certificadoId;
        public string NomeAluno { get; set; } = nomeAluno;
        public string NomeCurso { get; set; } = nomeCurso;
        public DateTime DataConclusao { get; set; } = dataConclusao;
        public string CodigoVerificacao { get; set; } = codigoVerificacao;

        public static CertificadoDTO FromMatricula(Matricula m) => new(m.Certificado!.Id,
             m.Aluno.Nome, m.NomeCurso, m.HistoricoAprendizado.DataConclusao!.Value,
             m.Certificado!.CodigoVerificacao);
    }
}