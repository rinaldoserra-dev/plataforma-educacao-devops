using PlataformaEducacao.GestaoAluno.Application.DTO;
using PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;
using PlataformaEducacao.GestaoAluno.Domain.Services;

namespace PlataformaEducacao.GestaoAluno.Application.Queries
{
    public class AlunoQueries : IAlunoQueries
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly ICertificadoService _certificadoService;

        public AlunoQueries(IAlunoRepository alunoRepository, ICertificadoService certificadoService)
        {
            _alunoRepository = alunoRepository;
            _certificadoService = certificadoService;
        }

        public async Task<IEnumerable<MatriculaPendentePagamentoDTO>> ListarMatriculasPendentesPagamentoPorAlunoId
            (Guid alunoId, CancellationToken cancellationToken)
        {
            var matriculas = await _alunoRepository.ListarMatriculasPendentesPagamentoPorAlunoId
                (alunoId, cancellationToken);

            return matriculas.Select(m => MatriculaPendentePagamentoDTO.FromMatricula(m));
        }

        public async Task<IEnumerable<MatriculaViewModel>> ObterAlunosMatriculadosPorCursoId(Guid cursoId, CancellationToken cancellationToken)
        {
            var matriculas = await _alunoRepository.ObterAlunosMatriculadosPorCursoId(cursoId, cancellationToken);

            return matriculas.Select(m => MatriculaViewModel.FromMatricula(m));
        }

        public async Task<IEnumerable<MatriculaViewModel>> ObterAlunosPendentesPorCursoId(Guid cursoId, CancellationToken cancellationToken)
        {
            var matriculas = await _alunoRepository.ObterAlunosPendentesPorCursoId(cursoId, cancellationToken);

            return matriculas.Select(m => MatriculaViewModel.FromMatricula(m));
        }

        public async Task<MatriculaViewModel?> ObterMatricula(Guid matriculaId, CancellationToken cancellationToken)
        {
            var matricula = await _alunoRepository.ObterMatriculaComAlunoPorId(matriculaId, cancellationToken);

            return matricula is null ? null : MatriculaViewModel.FromMatricula(matricula);
        }

        public async Task<IEnumerable<MatriculaAtivaDTO>> ObterMatriculasAtivasPorAlunoId(Guid alunoId, CancellationToken cancellationToken)
        {
            var matriculas = await _alunoRepository.ObterMatriculasAtivasPorAlunoId(alunoId, cancellationToken);

            return matriculas.Select(m => MatriculaAtivaDTO.FromMatricula(m));
        }

        public async Task<CertificadoDTO?> ValidarCertificado(string codigoVerificacao, CancellationToken cancellationToken)
        {
            var matricula = await _alunoRepository.ObterCertificadoPorCodigoVerificacao(codigoVerificacao, cancellationToken);
            if (matricula is null)
                return null;

            return matricula.Certificado is null ? null : CertificadoDTO.FromMatricula(matricula);
        }

        public async Task<ArquivoDTO?> BaixarCertificado(Guid certificadoId, CancellationToken cancellationToken)
        {
            var certificado = await _alunoRepository.ObterCertificadoPorCertificadoId(certificadoId, cancellationToken);
            if (certificado is null)
                return null;

            var certificadoArquivo = await _certificadoService.GerarCertificado(certificado!);

            return new ArquivoDTO
            {
                NomeArquivo = $"Certificado_{certificado.Matricula.Aluno.Nome}_{certificado.Matricula.NomeCurso}.pdf".Replace(" ", "_").Replace("/", "-"),
                ContentType = "application/pdf",
                PdfBytes = certificadoArquivo
            };
        }

        public async Task<HistoricoAlunoViewModel?> ObterHistoricoAluno(Guid alunoId, CancellationToken cancellationToken)
        {
            var alunoComMatriculas = await _alunoRepository.ObterComMatriculasPorId(alunoId, cancellationToken);

            return alunoComMatriculas is null ? null : HistoricoAlunoViewModel.FromAlunoComMatriculas(alunoComMatriculas);
        }
    }
}