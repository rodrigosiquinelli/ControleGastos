namespace ControleGastos.Application.DTOs.Pessoa
{
    public record PessoaDto
    {
        public Guid Id { get; init; }
        public string Nome { get; init; } = string.Empty;
        public DateTime DataNascimento { get; init; }
        public int Idade { get; init; }
    }
}
