using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.DTO
{
    public class MatriculaAtivaDTO(Guid matriculaId, Guid alunoId, string? nomeAluno, Guid cursoId,
        string nomeCurso, int situacaoMatricula, DateTime dataMatricula, int situacaoCurso,
        DateTime? dataConclusao, double progressoGeralCurso, Guid? certificadoId, string? codigoVerificacao)
    {
        public Guid MatriculaId { get; set; } = matriculaId;
        public Guid AlunoId { get; set; } = alunoId;
        public string NomeAluno { get; set; } = nomeAluno ?? string.Empty;
        public Guid CursoId { get; set; } = cursoId;
        public string NomeCurso { get; set; } = nomeCurso;
        public DateTime DataMatricula { get; set; } = dataMatricula;
        public int SituacaoMatricula { get; set; } = situacaoMatricula;
        public int SituacaoCurso { get; set; } = situacaoCurso;
        public DateTime? DataConclusao { get; set; } = dataConclusao;
        public double ProgressoGeralCurso { get; set; } = progressoGeralCurso;
        public Guid? CertificadoId { get; set; } = certificadoId;
        public string? CodigoVerificacao { get; set; } = codigoVerificacao;

        public static MatriculaAtivaDTO FromMatricula(Matricula m)
            => new(m.Id, m.AlunoId, m.Aluno?.Nome, m.CursoId, m.NomeCurso, (int)m.SituacaoMatricula,
                m.DataMatricula, (int)m.HistoricoAprendizado.SituacaoCurso,
                m.HistoricoAprendizado.DataConclusao, m.HistoricoAprendizado.ProgressoGeralCurso,
                m.Certificado?.Id, m.Certificado?.CodigoVerificacao);
    }
}