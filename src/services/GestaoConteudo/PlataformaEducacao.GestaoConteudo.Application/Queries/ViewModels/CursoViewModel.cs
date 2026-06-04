using PlataformaEducacao.GestaoConteudo.Domain;

namespace PlataformaEducacao.GestaoConteudo.Application.Queries.ViewModels
{
    public class CursoViewModel
    {
        public CursoViewModel(Guid id, string nome, string descricaoConteudo, int cargaHoraria, decimal valor, bool disponivel, IEnumerable<AulaViewModel> aulas)
        {
            Id = id;
            Nome = nome;
            DescricaoConteudo = descricaoConteudo;
            CargaHoraria = cargaHoraria;
            Valor = valor;
            Disponivel = disponivel;
            Aulas = aulas;
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string DescricaoConteudo { get; set; }
        public int CargaHoraria { get; set; }
        public decimal Valor { get; set; }
        public bool Disponivel { get; set; }
        public IEnumerable<AulaViewModel> Aulas { get; set; } = new List<AulaViewModel>();

        public static CursoViewModel FromCurso(Curso curso)
        => new(
            curso.Id,
            curso.Nome,
            curso.ConteudoProgramatico.Descricao,
            curso.ConteudoProgramatico.CargaHoraria,
            curso.Valor,
            curso.Disponivel,
            curso.Aulas == null ? Enumerable.Empty<AulaViewModel>() : curso.Aulas.OrderBy(a => a.Ordem).Select(AulaViewModel.FromAula)

        );
    }
}
