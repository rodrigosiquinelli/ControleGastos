namespace ControleGastos.Domain.DTOs
{
    public record RelatorioPessoasGeralDto
    {
        public IEnumerable<TotaisPessoaDto> Itens { get; init; } = new List<TotaisPessoaDto>();
        public decimal TotalGeralReceitas { get; init; }
        public decimal TotalGeralDespesas { get; init; }
        public decimal SaldoLiquidoGeral { get; init; }
    }
}
