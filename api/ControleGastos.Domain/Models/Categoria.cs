using ControleGastos.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Domain.Models
{
    // Entidade de domínio que representa uma categoria de despesas ou receitas, contendo regras de negócio e validações internas.
    public class Categoria : EntityBase
    {
        [Required]
        [StringLength(400)]
        public string Descricao { get; private set; } = string.Empty;

        [Required]
        public Finalidade Finalidade { get; private set; }

        // Propriedade de navegação utilizada pelo Entity Framework para gerenciar o relacionamento entre Categoria e Transações
        public ICollection<Transacao> Transacoes { get; } = new List<Transacao>();

        // Construtor vazio necessário para o funcionamento do Entity Framework
        protected Categoria() { }

        // Construtor principal que garante a criação de uma categoria em estado válido
        public Categoria(string descricao, Finalidade finalidade)
        {
            SetDescricao(descricao);
            SetFinalidade(finalidade);
        }

        // Altera a descrição validando se o valor não é nulo ou vazio antes da atribuição
        public void SetDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição é obrigatória.");
            Descricao = descricao;
        }

        // Define a finalidade garantindo que o valor recebido exista no enumerador correspondente
        public void SetFinalidade(Finalidade finalidade)
        {
            if (!Enum.IsDefined(typeof(Finalidade), finalidade))
                throw new ArgumentException("Finalidade inválida.");
            Finalidade = finalidade;
        }

        // Regra de negócio que verifica se o tipo da transação (Receita/Despesa) é compatível com a finalidade desta categoria
        public bool PermiteTipo(TipoTransacao tipo) =>
            Finalidade == Finalidade.Ambas || (int)Finalidade == (int)tipo;
    }
}
