namespace ControleGastos.Domain.DTOs
{
    public record TotaisCategoriaDto
    {
        public Guid CategoriaId { get; init; }
        public string Descricao { get; init; } = string.Empty;
        public decimal TotalReceitas { get; init; }
        public decimal TotalDespesas { get; init; }
        public decimal Saldo { get; init; }
    }
}
