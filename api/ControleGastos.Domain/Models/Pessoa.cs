namespace ControleGastos.Domain.Models
{
    // Entidade de domínio que representa uma pessoa, contendo regras de negócio e validações internas.

    public class Pessoa : EntityBase
    {
        public string Nome { get; private set; }
        public DateTime DataNascimento { get; private set; }

        // Propriedade calculada que determina a idade com base na data atual e data de nascimento.
        public int Idade
        {
            get
            {
                var hoje = DateTime.Today;
                var idade = hoje.Year - DataNascimento.Year;
                if (DataNascimento.Date > hoje.AddYears(-idade))
                    idade--;

                return idade;
            }
        }

        // Regra de negócio simples para identificar se a pessoa possui maioridade legal.
        public bool MaiorDeIdade => Idade >= 18;

        // Propriedade de navegação utilizada pelo Entity Framework para gerenciar o relacionamento entre Pessoa e Transações.
        public ICollection<Transacao> Transacoes { get; private set; } = new List<Transacao>();

        // Construtor vazio necessário para o funcionamento do Entity Framework
        protected Pessoa() { }

        // Construtor principal que garante a criação de uma pessoa em estado válido
        public Pessoa(string nome, DateTime dataNascimento)
        {
            Validar(nome, dataNascimento);
            Nome = nome;
            DataNascimento = dataNascimento;
        }

        // Atualiza o nome da pessoa garantindo que o novo valor passe pelas regras de validação.
        public void SetNome(string nome)
        {
            Validar(nome, DataNascimento);
            Nome = nome;
        }

        // Atualiza a data de nascimento da pessoa garantindo que o novo valor passe pelas regras de validação.
        public void SetDataNascimento(DateTime dataNascimento)
        {
            Validar(Nome, dataNascimento);
            DataNascimento = dataNascimento;
        }

        // Centraliza as regras de validação da entidade para evitar estados inválidos ou inconsistentes.
        private void Validar(string nome, DateTime dataNascimento)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome não pode ser vazio.");

            if (dataNascimento > DateTime.Today)
                throw new ArgumentException("A data de nascimento não pode ser no futuro.");
        }
    }
}
