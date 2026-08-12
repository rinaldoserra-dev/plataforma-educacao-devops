using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoConteudo.Application.Commands
{
    public class AdicionarCursoCommand : Command
    {
        public AdicionarCursoCommand(string nome, string descricaoConteudo, int cargaHoraria, decimal valor, bool disponivel)
        {
            Nome = nome;
            DescricaoConteudo = descricaoConteudo;
            CargaHoraria = cargaHoraria;
            Valor = valor;
            Disponivel = disponivel;
        }

        public string Nome { get; private set; }

        public string DescricaoConteudo { get; private set; }

        public int CargaHoraria { get; private set; }

        public decimal Valor { get; private set; }

        public bool Disponivel { get; private set; }

        public override bool EhValido()
        {
            ValidationResult = new AdicionarCursoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
