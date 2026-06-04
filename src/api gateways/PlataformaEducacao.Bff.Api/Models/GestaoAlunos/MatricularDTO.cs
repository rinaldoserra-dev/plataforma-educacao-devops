using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.Bff.Api.Models.GestaoAlunos
{
    public class MatricularDTO
    {
        [Required(ErrorMessage = "O id do curso é obrigatório.")]
        public Guid CursoId { get; set; }

        public string NomeCurso { get; set; } = string.Empty;

        public int TotalAulasCurso { get; set; }

        public decimal Valor { get; set; }
    }
}
