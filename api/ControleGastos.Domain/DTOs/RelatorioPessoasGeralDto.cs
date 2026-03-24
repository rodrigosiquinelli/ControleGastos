namespace ControleGastos.Domain.DTOs
{
    // Objeto imutável (record) que consolida os resultados financeiros de todas as pessoas para exibição em relatórios.
    public record RelatorioPessoasGeralDto
    {
        public IEnumerable<TotaisPessoaDto> Itens { get; init; } = new List<TotaisPessoaDto>();
        public decimal TotalGeralReceitas { get; init; }
        public decimal TotalGeralDespesas { get; init; }
        public decimal SaldoLiquidoGeral { get; init; }
    }
}
