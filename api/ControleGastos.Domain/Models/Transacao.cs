using ControleGastos.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleGastos.Domain.Models
{
    public class Transacao : EntityBase
    {
        [Required]
        [StringLength(400)]
        public string Descricao { get; private set; } = string.Empty;

        [Required]
        public decimal Valor { get; private set; }

        [Required]
        public TipoTransacao Tipo { get; private set; }

        [Required]
        public Guid CategoriaId { get; private set; }
        public virtual Categoria Categoria { get; private set; } = null!;

        [Required]
        public Guid PessoaId { get; private set; }
        public virtual Pessoa Pessoa { get; private set; } = null!;

        [Required]
        public DateTime Data { get; private set; }

        protected Transacao() { }

        public Transacao(string descricao, decimal valor, TipoTransacao tipo, Categoria categoria, Pessoa pessoa, DateTime data)
        {
            SetDescricao(descricao);
            SetValor(valor);
            SetTipo(tipo);
            SetData(data);
            SetCategoria(categoria);
            SetPessoa(pessoa);
        }

        public void SetDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao)) throw new ArgumentException("Descrição obrigatória.");
            Descricao = descricao;
        }

        public void SetValor(decimal valor)
        {
            if (valor <= 0) throw new ArgumentException("O valor deve ser positivo.");
            Valor = valor;
        }

        public void SetTipo(TipoTransacao tipo)
        {
            if (!Enum.IsDefined(typeof(TipoTransacao), tipo)) throw new ArgumentException("Tipo inválido.");
            Tipo = tipo;
        }

        public void SetData(DateTime data)
        {
            if (data > DateTime.Today) throw new ArgumentException("A data não pode ser futura.");
            Data = data;
        }

        public void SetCategoria(Categoria categoria)
        {
            if (categoria == null) throw new ArgumentNullException(nameof(categoria));

            if (!categoria.PermiteTipo(this.Tipo))
            {
                throw new InvalidOperationException(
                    this.Tipo == TipoTransacao.Despesa
                        ? "Não é possível registrar despesa em categoria de receita."
                        : "Não é possível registrar receita em categoria de despesa.");
            }

            Categoria = categoria;
            CategoriaId = categoria.Id;
        }

        public void SetPessoa(Pessoa pessoa)
        {
            if (pessoa == null) throw new ArgumentNullException(nameof(pessoa));

            if (this.Tipo == TipoTransacao.Receita && !pessoa.MaiorDeIdade)
            {
                throw new InvalidOperationException("Menores de 18 anos não podem registrar receitas.");
            }

            Pessoa = pessoa;
            PessoaId = pessoa.Id;
        }
    }
}
