using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.GestaoAluno.Domain.Events;

namespace PlataformaEducacao.GestaoAluno.Domain
{
    public class Matricula : Entity
    {
        public Guid CursoId { get; private set; }
        public string NomeCurso { get; private set; } = null!;
        public decimal Valor { get; private set; }
        public Guid AlunoId { get; private set; }
        public DateTime DataMatricula { get; private set; }
        public SituacaoMatricula SituacaoMatricula { get; private set; }
        public HistoricoAprendizado HistoricoAprendizado { get; private set; } = null!;
        public Aluno Aluno { get; private set; } = null!;
        public Certificado? Certificado { get; private set; }

        private readonly List<ProgressoAula> _progressoAulas;
        public IReadOnlyCollection<ProgressoAula> ProgressoAulas => _progressoAulas;

        protected Matricula()
        {
            _progressoAulas = [];
        }

        public Matricula(Guid cursoId, string nomeCurso, int totalAulasCurso, decimal valor)
        {
            CursoId = cursoId;
            NomeCurso = nomeCurso;
            Valor = valor;
            DataMatricula = DateTime.Now;
            SituacaoMatricula = SituacaoMatricula.PendentePagamento;
            HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.CriarInicial(totalAulasCurso);
            Certificado = null;
            _progressoAulas = [];

            Validar();
        }

        public void AssociarAluno(Guid alunoId)
        {
            AlunoId = alunoId;
        }

        public bool EstaAtiva()
        {
            return SituacaoMatricula == SituacaoMatricula.Ativa;
        }

        public void Ativar()
        {
            if (EstaAtiva())
                throw new DomainException("Matrícula ativa, não pode ser ativada novamente.");

            SituacaoMatricula = SituacaoMatricula.Ativa;

            AdicionarEvento(new MatriculaAtivadaEvent(Id));
        }

        public void Recusar()
        {
            if (EstaAtiva())
                throw new DomainException("Matrícula ativa, não pode ser recusada.");

            SituacaoMatricula = SituacaoMatricula.PendentePagamento;
        }

        public void IniciarPagamento()
        {
            if (SituacaoMatricula == SituacaoMatricula.Ativa)
                throw new DomainException("Matrícula paga, não pode iniciar pagamento novamente.");

            if (SituacaoMatricula == SituacaoMatricula.ProcessoPagamento)
                throw new DomainException("Matrícula em processo de pagamento, não pode iniciar pagamento novamente.");

            SituacaoMatricula = SituacaoMatricula.ProcessoPagamento;
        }

        public bool AulaRealizada(ProgressoAula progressoAula)
        {
            return _progressoAulas.Any(p => p.AulaId == progressoAula.AulaId);
        }

        public void RegistrarAula(ProgressoAula progressoAula)
        {
            if (EstaAtiva() is false)
                throw new DomainException("Não é permitido assistir aula antes do pagamento da matrícula.");

            if (AulaRealizada(progressoAula))
                throw new DomainException("Aula já realizada, não pode ser registrada novamente.");

            progressoAula.AssociarMatricula(Id);
            _progressoAulas.Add(progressoAula);

            AtualizaProgressoCurso();
        }

        private void AtualizaProgressoCurso()
        {
            var totalAulasCurso = HistoricoAprendizado.TotalAulasCurso;
            int aulasConcluidas = _progressoAulas.Count;

            double novoProgresso = (double)aulasConcluidas / totalAulasCurso * 100;

            HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.CriarEmAndamento(totalAulasCurso, novoProgresso);
        }

        public void FinalizarCurso()
        {
            if (EstaAtiva() is false)
                throw new DomainException("Antes de finalizar curso é obrigatório realizar pagamento da matrícula.");

            int aulasConcluidas = _progressoAulas.Count;
            int totalAulasCurso = HistoricoAprendizado.TotalAulasCurso;

            if (HistoricoAprendizado.ProgressoGeralCurso < 100 || aulasConcluidas != totalAulasCurso)
                throw new DomainException("Para finalizar curso é necessário assistir todas as aulas.");

            if (HistoricoAprendizado.SituacaoCurso == SituacaoCurso.Concluido)
                throw new DomainException("Curso já finalizado, não pode ser finalizado novamente.");

            double progresso = HistoricoAprendizado.ProgressoGeralCurso;

            HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.CriarFinalizado(totalAulasCurso, progresso);

            AdicionarEvento(new CursoFinalizadoEvent(Id));
        }

        public void GerarCertificado()
        {
            if (HistoricoAprendizado.SituacaoCurso != SituacaoCurso.Concluido)
                throw new DomainException("Para gerar certificado é necessário concluir o curso.");

            Certificado = new Certificado(Id);
        }

        protected void Validar()
        {
            Validacoes.ValidarSeVazio(CursoId, "O id do curso é obrigatório.");
            Validacoes.ValidarSeVazio(NomeCurso, "O nome do curso é obrigatório.");
            Validacoes.ValidarSeMenorOuIgualQue(Valor, 0, "O valor do curso deve ser maior que zero.");
        }

        public static class MatriculaFactory
        {
            public static Matricula CriarComPagamentoAprovado(Guid cursoId, string nomeCurso, int totalAulasCurso, decimal valor, Aluno aluno)
            {
                var matricula = new Matricula
                {
                    Aluno = aluno,
                    AlunoId = aluno.Id,
                    NomeCurso = nomeCurso,
                    CursoId = cursoId,
                    HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.CriarEmAndamento(totalAulasCurso, progressoGeral: 0),
                    SituacaoMatricula = SituacaoMatricula.Ativa,
                    DataMatricula = DateTime.Now,
                    Valor = valor
                };

                matricula.Validar();

                return matricula;
            }

            public static Matricula CriarComCursoFinalizado(Guid cursoId, string nomeCurso, int totalAulasCurso, decimal valor, Aluno aluno)
            {
                var matricula = new Matricula
                {
                    Aluno = aluno,
                    AlunoId = aluno.Id,
                    NomeCurso = nomeCurso,
                    CursoId = cursoId,
                    HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.CriarEmAndamento(totalAulasCurso, progressoGeral: 100),
                    SituacaoMatricula = SituacaoMatricula.Ativa,
                    DataMatricula = DateTime.Now,
                    Valor = valor
                };

                matricula.Validar();
                matricula.RegistrarAula(new ProgressoAula(Guid.NewGuid()));
                matricula.FinalizarCurso();

                return matricula;
            }

            public static Matricula CriarComPagamentoEmProcessamento(Guid cursoId, string nomeCurso, int totalAulasCurso, decimal valor, Aluno aluno)
            {
                var matricula = new Matricula
                {
                    Aluno = aluno,
                    AlunoId = aluno.Id,
                    NomeCurso = nomeCurso,
                    CursoId = cursoId,
                    HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.CriarEmAndamento(totalAulasCurso, progressoGeral: 0),
                    SituacaoMatricula = SituacaoMatricula.ProcessoPagamento,
                    DataMatricula = DateTime.Now,
                    Valor = valor
                };

                matricula.Validar();

                return matricula;
            }
        }
    }
}