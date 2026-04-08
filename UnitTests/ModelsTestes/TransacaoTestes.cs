using Bogus;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Models;
using FluentAssertions;

namespace UnitTestes.ModelsTestes
{
    public class TransacaoTestes
    {
        private readonly Faker _faker = new("pt_BR");

        private Pessoa CriarPessoa(int idade) =>
            new(_faker.Person.FullName, DateTime.Today.AddYears(-idade));

        private Categoria CriarCategoria(Finalidade finalidade) =>
            new(_faker.Commerce.Categories(1)[0], finalidade);

        public class Construtor : TransacaoTestes
        {
            [Fact]
            public void Deve_Criar_Transacao_Quando_Dados_Forem_Validos()
            {
                var pessoa = CriarPessoa(20);
                var categoria = CriarCategoria(Finalidade.Despesa);
                var descricao = _faker.Commerce.ProductDescription();
                var valor = _faker.Random.Decimal(1, 1000);
                var data = DateTime.Today;

                var transacao = new Transacao(descricao, valor, TipoTransacao.Despesa, categoria, pessoa, data);

                transacao.Descricao.Should().Be(descricao);
                transacao.Valor.Should().Be(valor);
                transacao.Tipo.Should().Be(TipoTransacao.Despesa);
                transacao.Pessoa.Should().Be(pessoa);
                transacao.Categoria.Should().Be(categoria);
            }
        }
        
        public class RegrasDeNegocio : TransacaoTestes
        {
            [Fact]
            public void Deve_Lancar_Excecao_Quando_Categoria_Nao_Permite_Tipo_Transacao()
            {
                var categoriaReceita = CriarCategoria(Finalidade.Receita);
                var pessoa = CriarPessoa(20);

                Action acao = () => new Transacao("Teste", 100, TipoTransacao.Despesa, categoriaReceita, pessoa, DateTime.Today);

                acao.Should().Throw<InvalidOperationException>()
                    .WithMessage("Não é possível registrar despesa em categoria de receita.");
            }

            [Fact]
            public void Deve_Lancar_Excecao_Quando_Menor_De_Idade_Tenta_Registrar_Receita()
            {
                var pessoaMenor = CriarPessoa(17);
                var categoria = CriarCategoria(Finalidade.Receita);

                Action acao = () => new Transacao("Receita", 100, TipoTransacao.Receita, categoria, pessoaMenor, DateTime.Today);

                acao.Should().Throw<InvalidOperationException>()
                    .WithMessage("Menores de 18 anos não podem registrar receitas.");
            }

            [Fact]
            public void Deve_Permitir_Receita_Para_Maior_De_Idade()
            {
                var pessoaMaior = CriarPessoa(18);
                var categoria = CriarCategoria(Finalidade.Receita);

                var transacao = new Transacao("Salário", 1000, TipoTransacao.Receita, categoria, pessoaMaior, DateTime.Today);

                transacao.Tipo.Should().Be(TipoTransacao.Receita);
                transacao.Pessoa.MaiorDeIdade.Should().BeTrue();
            }
        }

        public class Modificadores : TransacaoTestes
        {
            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            [InlineData(null)]
            public void Deve_Lancar_Excecao_Ao_Setar_Descricao_Invalida(string descricaoInvalida)
            {
                var transacao = new Transacao("Original", 10, TipoTransacao.Despesa, CriarCategoria(Finalidade.Despesa), CriarPessoa(20), DateTime.Today);

                Action acao = () => transacao.SetDescricao(descricaoInvalida);

                acao.Should().Throw<ArgumentException>().WithMessage("Descrição obrigatória.");
            }

            [Fact]
            public void Deve_Lancar_Excecao_Ao_Setar_Valor_Negativo_Ou_Zero()
            {
                var transacao = new Transacao("Teste", 10, TipoTransacao.Despesa, CriarCategoria(Finalidade.Despesa), CriarPessoa(20), DateTime.Today);

                Action acaoZero = () => transacao.SetValor(0);
                Action acaoNegativo = () => transacao.SetValor(-1);

                acaoZero.Should().Throw<ArgumentException>().WithMessage("O valor deve ser positivo.");
                acaoNegativo.Should().Throw<ArgumentException>().WithMessage("O valor deve ser positivo.");
            }

            [Fact]
            public void Deve_Lancar_Excecao_Ao_Setar_Data_Futura()
            {
                var transacao = new Transacao("Teste", 10, TipoTransacao.Despesa, CriarCategoria(Finalidade.Despesa), CriarPessoa(20), DateTime.Today);
                var dataFutura = DateTime.Today.AddDays(1);

                Action acao = () => transacao.SetData(dataFutura);

                acao.Should().Throw<ArgumentException>().WithMessage("A data não pode ser futura.");
            }

            [Fact]
            public void Deve_Lancar_Excecao_Ao_Setar_Tipo_Invalido()
            {
                var transacao = new Transacao("Teste", 10, TipoTransacao.Despesa, CriarCategoria(Finalidade.Despesa), CriarPessoa(20), DateTime.Today);

                Action acao = () => transacao.SetTipo((TipoTransacao)99);

                acao.Should().Throw<ArgumentException>().WithMessage("Tipo inválido.");
            }
        }
    }
}
