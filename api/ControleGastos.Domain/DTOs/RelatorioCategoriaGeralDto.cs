namespace ControleGastos.Domain.DTOs
{
    // Objeto imutável (record) que consolida os resultados financeiros de todas as categorias para exibição em relatórios.
    public record RelatorioCategoriaGeralDto
    {
        public IEnumerable<TotaisCategoriaDto> Categorias { get; init; } = Enumerable.Empty<TotaisCategoriaDto>();
        public decimal TotalGeralReceitas { get; init; }
        public decimal TotalGeralDespesas { get; init; }
        public decimal SaldoLiquidoGeral { get; init; }
    }
}
