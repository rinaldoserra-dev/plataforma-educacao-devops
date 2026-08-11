using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.GerarCertificado
{
    public class GerarCertificadoCommand : Command
    {
        public Guid MatriculaId { get; private set; }

        public GerarCertificadoCommand(Guid matriculaId)
        {
            MatriculaId = matriculaId;
        }
    }
}
