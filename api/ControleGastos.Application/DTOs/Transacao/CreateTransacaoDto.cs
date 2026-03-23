using ControleGastos.Application.DTOs.Validations;
using ControleGastos.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Application.DTOs.Transacao
{
    public record CreateTransacaoDto
    {
        [Required(ErrorMessage = "Descrição é obrigatória.")]
        [StringLength(200, ErrorMessage = "Descrição muito longa.")]
        public string Descricao { get; init; } = string.Empty;

        [Required(ErrorMessage = "Valor é obrigatório.")]
        [PositiveValue(ErrorMessage = "O valor deve ser positivo.")]
        public decimal Valor { get; init; }

        [Required(ErrorMessage = "Tipo é obrigatório.")]
        public TipoTransacao Tipo { get; init; }

        [Required(ErrorMessage = "Categoria é obrigatória.")]
        public Guid CategoriaId { get; init; }

        [Required(ErrorMessage = "Pessoa é obrigatória.")]
        public Guid PessoaId { get; init; }

        [Required(ErrorMessage = "Data é obrigatória.")]
        [PastDate(ErrorMessage = "A transação não pode ser em data futura.")]
        public DateTime Data { get; init; }
    }
}
