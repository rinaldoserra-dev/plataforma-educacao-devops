using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels
{
    public class MatriculaViewModel
    {
        public MatriculaViewModel(Guid matriculaId, Guid alunoId, string? nomeAluno, Guid cursoId, string nomeCurso, SituacaoMatricula situacaoMatricula, DateTime dataMatricula, SituacaoCurso situacaoCurso, DateTime? dataConclusao, double progressoGeralCurso, Guid? certificadoId, string? codigoVerificacao)
        {
            MatriculaId = matriculaId;
            AlunoId = alunoId;
            NomeAluno = nomeAluno ?? string.Empty;
            CursoId = cursoId;
            NomeCurso = nomeCurso;
            SituacaoMatricula = situacaoMatricula;
            DataMatricula = dataMatricula;
            SituacaoCurso = situacaoCurso;
            DataConclusao = dataConclusao;
            ProgressoGeralCurso = progressoGeralCurso;
            CertificadoId = certificadoId;
            CodigoVerificacao = codigoVerificacao;
        }

        public Guid MatriculaId { get; set; }
        public Guid AlunoId { get; set; }
        public string NomeAluno { get; set; } = null!;
        public Guid CursoId { get; set; }
        public string NomeCurso { get; set; } = null!;
        public SituacaoMatricula SituacaoMatricula { get; set; }
        public SituacaoCurso SituacaoCurso { get; set; }
        public DateTime? DataConclusao { get; set; }
        public double ProgressoGeralCurso { get; set; }
        public DateTime DataMatricula { get; set; }
        public Guid? CertificadoId { get; set; }
        public string? CodigoVerificacao { get; set; }

        public static MatriculaViewModel FromMatricula(Matricula matricula)
         => new(
             matricula.Id,
             matricula.AlunoId,
             matricula.Aluno?.Nome,
             matricula.CursoId,
             matricula.NomeCurso,
             matricula.SituacaoMatricula,
             matricula.DataMatricula,
             matricula.HistoricoAprendizado.SituacaoCurso,
             matricula.HistoricoAprendizado.DataConclusao,
             matricula.HistoricoAprendizado.ProgressoGeralCurso,
             matricula.Certificado?.Id,
             matricula.Certificado?.CodigoVerificacao
         );
    }
}
