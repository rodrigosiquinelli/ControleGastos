using Bogus;
using ControleGastos.Domain.Models;
using FluentAssertions;

namespace UnitTestes.ModelsTestes
{
    public class PessoaTestes
    {
        private readonly Faker _faker = new("pt_BR");

        public class Construtor : PessoaTestes
        {
            [Fact]
            public void Deve_Criar_Pessoa_Quando_Dados_Forem_Validos()
            {
                var nome = _faker.Person.FullName;
                var dataNascimento = _faker.Date.Past(20);

                var pessoa = new Pessoa(nome, dataNascimento);

                pessoa.Nome.Should().Be(nome);
                pessoa.DataNascimento.Should().Be(dataNascimento);
            }

            [Theory]
            [InlineData("")]
            [InlineData("Nome Valido")]
            public void Deve_Lancar_Excecao_Quando_Dados_Forem_Invalidos(string nome)
            {
                var dataNascimento = nome == "" ? DateTime.Today.AddYears(-20) : DateTime.Today.AddDays(1);

                Action acao = () => new Pessoa(nome, dataNascimento);

                acao.Should().Throw<ArgumentException>();
            }
        }

        public class RegrasDeNegocio : PessoaTestes
        {
            [Fact]
            public void Deve_Calcular_Idade_Corretamente()
            {
                var hoje = DateTime.Today;
                var dataNascimento = new DateTime(hoje.Year - 25, hoje.Month, hoje.Day);
                var pessoa = new Pessoa("Teste", dataNascimento);

                pessoa.Idade.Should().Be(25);
            }

            [Theory]
            [InlineData(18, true)]
            [InlineData(17, false)]
            public void Deve_Identificar_Maior_De_Idade(int idade, bool esperado)
            {
                var dataNascimento = DateTime.Today.AddYears(-idade);
                var pessoa = new Pessoa("Teste", dataNascimento);

                pessoa.MaiorDeIdade.Should().Be(esperado);
            }
        }

        public class Modificadores : PessoaTestes
        {
            [Fact]
            public void Deve_Atualizar_Nome_Quando_Valido()
            {
                var pessoa = new Pessoa("Nome Antigo", DateTime.Today.AddYears(-20));
                var novoNome = _faker.Person.FullName;

                pessoa.SetNome(novoNome);

                pessoa.Nome.Should().Be(novoNome);
            }

            [Fact]
            public void Deve_Lancar_Excecao_Ao_Atualizar_Dados_Invalidos()
            {
                var pessoa = new Pessoa("Teste", DateTime.Today.AddYears(-20));

                Action acao = () => pessoa.SetNome(string.Empty);

                acao.Should().Throw<ArgumentException>();
            }

            [Fact]
            public void Deve_Atualizar_DataNascimento_Quando_Valida()
            {
                var pessoa = new Pessoa("Teste", DateTime.Today.AddYears(-20));
                var novaData = DateTime.Today.AddYears(-30);

                pessoa.SetDataNascimento(novaData);

                pessoa.DataNascimento.Should().Be(novaData);
            }

            [Fact]
            public void Deve_Lancar_Excecao_Ao_Atualizar_DataNascimento_Futura()
            {
                var pessoa = new Pessoa("Teste", DateTime.Today.AddYears(-20));
                var dataFutura = DateTime.Today.AddDays(1);

                Action acao = () => pessoa.SetDataNascimento(dataFutura);

                acao.Should().Throw<ArgumentException>()
                    .WithMessage("A data de nascimento não pode ser no futuro.");
            }
        }
    }
}
