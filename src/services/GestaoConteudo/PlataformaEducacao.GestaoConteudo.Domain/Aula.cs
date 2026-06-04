using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoConteudo.Domain
{
    public class Aula : Entity
    {
        public string Titulo { get; private set; } = null!;
        public string Conteudo { get; private set; } = null!;
        public int Ordem { get; private set; }
        public string? Material { get; private set; }
        public Guid CursoId { get; private set; }
        public Curso Curso { get; private set; } = null!;

        protected Aula() { }
        public Aula(string titulo, string conteudo, int ordem, string? material)
        {
            Titulo = titulo;
            Conteudo = conteudo;
            Ordem = ordem;
            Material = material;

            Validar();
        }
        public void VincularCurso(Guid cursoId)
        {
            CursoId = cursoId;
            Validacoes.ValidarSeVazio(CursoId, "O curso não pode ser vazio");
        }

        private void Validar()
        {
            Validacoes.ValidarSeVazio(Titulo, "O título da aula é orbigatório.");
            Validacoes.ValidarSeVazio(Conteudo, "O conteudo da aula é obrigatório.");
            Validacoes.ValidarSeMenorOuIgualQue(Ordem, 0, "A ordem da aula deve ser maior que 0.");
        }
    }
}
