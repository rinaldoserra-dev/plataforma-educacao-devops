using PlataformaEducacao.GestaoConteudo.Application.Queries.ViewModels;
using PlataformaEducacao.GestaoConteudo.Domain;
using PlataformaEducacao.GestaoConteudo.Domain.ValueObjects;

namespace PlataformaEducacao.GestaoConteudo.Application.Tests.Queries.ViewModels
{
    public class CursoViewModelTests
    {
        [Fact(DisplayName = "FromCurso deve mapear propriedades corretamente e ordenar aulas por Ordem")]
        [Trait("Categoria", "Gestao Conteudo - ViewModels - CursoViewModel")]
        public void FromCurso_MapeiaPropriedadesEOrdenaAulas()
        {
            // Arrange
            var conteudo = new ConteudoProgramatico("Descricao do conteudo", 120);
            var curso = new Curso("Curso Teste", conteudo, 199.9m, true);

            var aula1 = new Aula("Aula 1", "Conteudo 1", 2, "Material 1");
            var aula2 = new Aula("Aula 2", "Conteudo 2", 1, "Material 2");

            curso.AdicionarAula(aula1);
            curso.AdicionarAula(aula2);

            // Act
            var vm = CursoViewModel.FromCurso(curso);

            // Assert
            Assert.NotNull(vm);
            Assert.Equal(curso.Id, vm.Id);
            Assert.Equal(curso.Nome, vm.Nome);
            Assert.Equal(conteudo.Descricao, vm.DescricaoConteudo);
            Assert.Equal(conteudo.CargaHoraria, vm.CargaHoraria);
            Assert.Equal(curso.Valor, vm.Valor);
            Assert.Equal(curso.Disponivel, vm.Disponivel);

            Assert.Equal(2, vm.Aulas.Count());
            // Aulas devem estar ordenadas por Ordem (1 then 2)
            Assert.Equal(1, vm.Aulas.ElementAt(0).Ordem);
            Assert.Equal(aula2.Titulo, vm.Aulas.ElementAt(0).Titulo);
            Assert.Equal(2, vm.Aulas.ElementAt(1).Ordem);
        }

        [Fact(DisplayName = "FromCurso Quando curso não possui aulas Deve retornar lista vazia de aulas")]
        [Trait("Categoria", "Gestao Conteudo - ViewModels - CursoViewModel")]
        public void FromCurso_SemAulas_RetornaListaVazia()
        {
            // Arrange
            var conteudo = new ConteudoProgramatico("Descricao do conteudo", 80);
            var curso = new Curso("Curso Sem Aulas", conteudo, 99.9m, false);

            // Act
            var vm = CursoViewModel.FromCurso(curso);

            // Assert
            Assert.NotNull(vm);
            Assert.Empty(vm.Aulas);
        }
    }
}
