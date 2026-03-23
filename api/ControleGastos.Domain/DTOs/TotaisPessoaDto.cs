namespace ControleGastos.Domain.DTOs
{
    public record TotaisPessoaDto
    {
        public Guid PessoaId { get; init; }
        public string Nome { get; init; } = string.Empty;
        public decimal TotalReceitas { get; init; }
        public decimal TotalDespesas { get; init; }
        public decimal Saldo { get; init; }
    }
}
