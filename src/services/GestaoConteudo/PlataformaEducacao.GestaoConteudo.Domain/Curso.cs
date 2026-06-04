using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;

namespace PlataformaEducacao.GestaoConteudo.Domain
{
    public class Curso : Entity, IAggregateRoot
    {
        public string Nome { get; private set; } = null!;
        public ConteudoProgramatico ConteudoProgramatico { get; private set; } = null!;
        public decimal Valor { get; private set; }
        public bool Disponivel { get; private set; }

        private readonly List<Aula> _aulas;
        public IReadOnlyCollection<Aula> Aulas => _aulas;

        protected Curso()
        {
            _aulas = new List<Aula>();
        }

        public Curso(string nome, ConteudoProgramatico conteudoProgramatico, decimal valor, bool disponivel)
        {
            Nome = nome;
            ConteudoProgramatico = conteudoProgramatico;
            Valor = valor;
            Disponivel = disponivel;
            _aulas = new List<Aula>();

            Validar();
        }

        public void AtualizarNome(string nome)
        {
            Nome = nome;

            Validar();
        }

        public void AtualizarValor(decimal valor)
        {
            Valor = valor;

            Validar();
        }

        public void TornarDisponivel()
        {
            Disponivel = true;
        }
        public void TornarIndisponivel()
        {
            Disponivel = false;
        }

        public void AtualizarConteudoProgramatico(ConteudoProgramatico conteudoProgramatico)
        {
            ConteudoProgramatico = conteudoProgramatico;
        }

        public void AdicionarAula(Aula aula)
        {
            if (AulaExistente(aula))
                throw new DomainException("Aula já associada a este curso.");
            aula.VincularCurso(Id);
            _aulas.Add(aula);
        }

        public bool AulaExistente(Aula aula)
        {
            return _aulas.Any(a => a.Titulo == aula.Titulo);
        }

        private void Validar()
        {
            Validacoes.ValidarSeVazio(Nome, "O nome do curso é obrigatóro.");
            Validacoes.ValidarSeMenorOuIgualQue(Valor, 0, "O Valor do curso deve ser maior que 0.");
        }
    }
}
