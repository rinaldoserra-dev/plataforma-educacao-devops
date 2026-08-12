using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoConteudo.Application.Commands
{
    public class AdicionarAulaCommand : Command
    {
        public AdicionarAulaCommand(string titulo, string conteudo, int ordem, string? material, Guid cursoId)
        {
            Titulo = titulo;
            Conteudo = conteudo;
            Ordem = ordem;
            Material = material;
            CursoId = cursoId;
        }

        public Guid CursoId { get; private set; }

        public string Titulo { get; private set; }

        public string Conteudo { get; private set; }

        public int Ordem { get; private set; }

        public string? Material { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarAulaCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
