using Bogus;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Models;
using FluentAssertions;

namespace UnitTestes.ModelsTestes
{
    public class CategoriaTestes
    {
        private readonly Faker _faker = new("pt_BR");

        public class Construtor : CategoriaTestes
        {
            [Fact]
            public void Deve_Criar_Categoria_Quando_Dados_Forem_Validos()
            {
                var descricao = _faker.Commerce.Categories(1)[0];
                var finalidade = Finalidade.Receita;

                var categoria = new Categoria(descricao, finalidade);

                categoria.Descricao.Should().Be(descricao);
                categoria.Finalidade.Should().Be(finalidade);
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void Deve_Lancar_Excecao_Quando_Descricao_For_Invalida(string descricaoInvalida)
            {
                Action acao = () => new Categoria(descricaoInvalida, Finalidade.Despesa);

                acao.Should().Throw<ArgumentException>().WithMessage("A descrição é obrigatória.");
            }
        }

        public class Modificadores : CategoriaTestes
        {
            [Fact]
            public void Deve_Atualizar_Descricao_Quando_Valida()
            {
                var categoria = new Categoria("Antiga", Finalidade.Ambas);
                var novaDescricao = _faker.Commerce.Categories(1)[0];

                categoria.SetDescricao(novaDescricao);

                categoria.Descricao.Should().Be(novaDescricao);
            }

            [Fact]
            public void Deve_Atualizar_Finalidade_Quando_Valida()
            {
                var categoria = new Categoria("Teste", Finalidade.Despesa);

                categoria.SetFinalidade(Finalidade.Receita);

                categoria.Finalidade.Should().Be(Finalidade.Receita);
            }

            [Fact]
            public void Deve_Lancar_Excecao_Ao_Setar_Finalidade_Invalida()
            {
                var categoria = new Categoria("Teste", Finalidade.Despesa);

                Action acao = () => categoria.SetFinalidade((Finalidade)99);

                acao.Should().Throw<ArgumentException>().WithMessage("Finalidade inválida.");
            }
        }

        public class RegrasDeNegocio : CategoriaTestes
        {
            [Theory]
            [InlineData(Finalidade.Receita, TipoTransacao.Receita, true)]
            [InlineData(Finalidade.Receita, TipoTransacao.Despesa, false)]
            [InlineData(Finalidade.Despesa, TipoTransacao.Despesa, true)]
            [InlineData(Finalidade.Despesa, TipoTransacao.Receita, false)]
            [InlineData(Finalidade.Ambas, TipoTransacao.Receita, true)]
            [InlineData(Finalidade.Ambas, TipoTransacao.Despesa, true)]
            public void Deve_Validar_Se_Permite_Tipo_Transacao_Corretamente(Finalidade finalidade, TipoTransacao tipo, bool esperado)
            {
                var categoria = new Categoria("Teste", finalidade);

                var resultado = categoria.PermiteTipo(tipo);

                resultado.Should().Be(esperado);
            }
        }
    }
}
