using ControleGastos.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Domain.Models
{
    public class Categoria : EntityBase
    {
        [Required]
        [StringLength(400)]
        public string Descricao { get; private set; } = string.Empty;

        [Required]
        public Finalidade Finalidade { get; private set; }

        public ICollection<Transacao> Transacoes { get; } = new List<Transacao>();

        protected Categoria() { }

        public Categoria(string descricao, Finalidade finalidade)
        {
            SetDescricao(descricao);
            SetFinalidade(finalidade);
        }

        public void SetDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição é obrigatória.");
            Descricao = descricao;
        }

        public void SetFinalidade(Finalidade finalidade)
        {
            if (!Enum.IsDefined(typeof(Finalidade), finalidade))
                throw new ArgumentException("Finalidade inválida.");
            Finalidade = finalidade;
        }

        public bool PermiteTipo(TipoTransacao tipo) =>
            Finalidade == Finalidade.Ambas || (int)Finalidade == (int)tipo;
    }
}
