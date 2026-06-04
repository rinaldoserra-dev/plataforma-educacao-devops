

using PlataformaEducacao.Core;

namespace PlataformaEducacao.GestaoConteudo.Domain.ValueObjects
{
    public class ConteudoProgramatico
    {
        public string Descricao { get; private set; } = null!;
        public int CargaHoraria { get; private set; }


        public ConteudoProgramatico(string descricao, int cargaHoraria)
        {
            Descricao = descricao;
            CargaHoraria = cargaHoraria;

            Validar();
        }

        private void Validar()
        {
            Validacoes.ValidarSeVazio(Descricao, "A descrição do conteudo programático é obrigatória.");
            Validacoes.ValidarSeMenorOuIgualQue(CargaHoraria, 0, "Carga Horaria do curso deve ser maior que 0.");
        }
    }
}
