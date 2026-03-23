using ControleGastos.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Application.DTOs.Categoria
{
    public record CreateCategoriaDto
    {
        [Required(ErrorMessage = "Descrição é obrigatória.")]
        [StringLength(200, ErrorMessage = "Descrição pode ter no máximo 200 caracteres.")]
        public string Descricao { get; init; } = string.Empty;

        [Required(ErrorMessage = "Finalidade é obrigatória.")]
        public Finalidade Finalidade { get; init; }
    }
}
