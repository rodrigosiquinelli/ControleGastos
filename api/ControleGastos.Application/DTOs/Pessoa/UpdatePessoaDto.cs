using ControleGastos.Application.DTOs.Validations;
using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Application.DTOs.Pessoa
{
    public record UpdatePessoaDto
    {
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres.")]
        public string Nome { get; init; } = string.Empty;

        [Required(ErrorMessage = "Data de nascimento é obrigatória.")]
        [PastDate(ErrorMessage = "A data de nascimento não pode ser maior que hoje.")]
        public DateTime DataNascimento { get; init; }
    }
}
