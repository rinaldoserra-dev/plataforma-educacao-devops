using PlataformaEducacao.GestaoConteudo.Domain;

namespace PlataformaEducacao.GestaoConteudo.Application.Queries.ViewModels
{
    public class AulaViewModel
    {
        public AulaViewModel(Guid id, Guid cursoId, string titulo, string conteudo, int ordem, string? material)
        {
            Id = id;
            CursoId = cursoId;
            Titulo = titulo;
            Conteudo = conteudo;
            Ordem = ordem;
            Material = material;
        }

        public Guid Id { get; set; }
        public Guid CursoId { get; private set; }
        public string Titulo { get; private set; }
        public string Conteudo { get; private set; }
        public int Ordem { get; private set; }
        public string? Material { get; set; }

        public static AulaViewModel FromAula(Aula aula)
        => new(
            aula.Id,
            aula.CursoId,
            aula.Titulo,
            aula.Conteudo,
            aula.Ordem,
            aula.Material
        );
    }
}
