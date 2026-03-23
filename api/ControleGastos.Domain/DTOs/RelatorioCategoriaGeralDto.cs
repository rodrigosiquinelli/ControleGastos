namespace ControleGastos.Domain.DTOs
{
    public record RelatorioCategoriaGeralDto
    {
        public IEnumerable<TotaisCategoriaDto> Categorias { get; init; } = Enumerable.Empty<TotaisCategoriaDto>();
        public decimal TotalGeralReceitas { get; init; }
        public decimal TotalGeralDespesas { get; init; }
        public decimal SaldoLiquidoGeral { get; init; }
    }
}
