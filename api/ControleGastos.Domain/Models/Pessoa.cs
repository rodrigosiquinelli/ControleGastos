using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Domain.Models
{
    public class Pessoa
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public DateTime DataNascimento { get; private set; }

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

        public bool MaiorDeIdade => Idade >= 18;

        public ICollection<Transacao> Transacoes { get; private set; } = new List<Transacao>();

        protected Pessoa() { }

        public Pessoa(string nome, DateTime dataNascimento)
        {
            Validar(nome, dataNascimento);
            Nome = nome;
            DataNascimento = dataNascimento;
        }

        public void SetNome(string nome)
        {
            Validar(nome, DataNascimento);
            Nome = nome;
        }

        public void SetDataNascimento(DateTime dataNascimento)
        {
            Validar(Nome, dataNascimento);
            DataNascimento = dataNascimento;
        }

        private void Validar(string nome, DateTime dataNascimento)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome não pode ser vazio.");

            if (dataNascimento > DateTime.Today)
                throw new ArgumentException("A data de nascimento não pode ser no futuro.");
        }
    }
}
