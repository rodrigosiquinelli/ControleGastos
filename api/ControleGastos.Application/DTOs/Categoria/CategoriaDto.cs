using ControleGastos.Domain.Enums;

namespace ControleGastos.Application.DTOs.Categoria
{
    public record CategoriaDto
    {
        public Guid Id { get; init; }
        public string Descricao { get; init; } = string.Empty;
        public Finalidade Finalidade { get; init; }
    }
}
