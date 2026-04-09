using AutoMapper;
using Bogus;
using ControleGastos.Application.DTOs.Transacao;
using ControleGastos.Application.Services;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;
using FluentAssertions;
using Moq;

namespace UnitTestes.ServicesTestes
{
    public class TransacaoServiceTestes
    {
        private readonly Mock<ITransacaoRepository> _repositoryMock;
        private readonly Mock<IPessoaRepository> _pessoaRepositoryMock;
        private readonly Mock<ICategoriaRepository> _categoriaRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TransacaoService _service;
        private readonly Faker _faker;

        public TransacaoServiceTestes()
        {
            _repositoryMock = new Mock<ITransacaoRepository>();
            _pessoaRepositoryMock = new Mock<IPessoaRepository>();
            _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _faker = new Faker("pt_BR");

            _service = new TransacaoService(
                _repositoryMock.Object,
                _pessoaRepositoryMock.Object,
                _categoriaRepositoryMock.Object,
                _uowMock.Object,
                _mapperMock.Object);
        }

        public class CriarAsync : TransacaoServiceTestes
        {
            [Fact]
            public async Task Deve_Criar_Transacao_Com_Sucesso()
            {
                var pessoa = new Pessoa(_faker.Person.FullName, DateTime.Today.AddYears(-20));
                var categoria = new Categoria(_faker.Commerce.Categories(1)[0], Finalidade.Ambas);
                var dto = new CreateTransacaoDto
                {
                    PessoaId = Guid.NewGuid(),
                    CategoriaId = Guid.NewGuid(),
                    Descricao = _faker.Commerce.ProductDescription(),
                    Valor = 100,
                    Tipo = TipoTransacao.Despesa,
                    Data = DateTime.Today
                };

                _pessoaRepositoryMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync(pessoa);
                _categoriaRepositoryMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(categoria);
                _mapperMock.Setup(m => m.Map<TransacaoDto>(It.IsAny<Transacao>())).Returns(new TransacaoDto());

                var resultado = await _service.CreateAsync(dto);

                resultado.Should().NotBeNull();
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Transacao>()), Times.Once);
                _uowMock.Verify(u => u.CommitAsync(), Times.Once);
            }

            [Fact]
            public async Task Deve_Lancar_Excecao_Quando_Pessoa_Nao_Encontrada()
            {
                var dto = new CreateTransacaoDto { PessoaId = Guid.NewGuid() };
                _pessoaRepositoryMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync((Pessoa)null!);

                Func<Task> acao = async () => await _service.CreateAsync(dto);

                await acao.Should().ThrowAsync<Exception>().WithMessage("Pessoa não encontrada.");
                _uowMock.Verify(u => u.CommitAsync(), Times.Never);
            }

            [Fact]
            public async Task Deve_Lancar_Excecao_Quando_Categoria_Nao_Encontrada()
            {
                var dto = new CreateTransacaoDto { PessoaId = Guid.NewGuid(), CategoriaId = Guid.NewGuid() };
                _pessoaRepositoryMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync(new Pessoa("Teste", DateTime.Today.AddYears(-20)));
                _categoriaRepositoryMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync((Categoria)null!);

                Func<Task> acao = async () => await _service.CreateAsync(dto);

                await acao.Should().ThrowAsync<Exception>().WithMessage("Categoria não encontrada.");
                _uowMock.Verify(u => u.CommitAsync(), Times.Never);
            }

            [Theory]
            [InlineData(0, TipoTransacao.Despesa, 20, Finalidade.Despesa)] // Valor zero
            [InlineData(100, TipoTransacao.Receita, 15, Finalidade.Receita)] // Menor de idade com receita
            [InlineData(100, TipoTransacao.Despesa, 20, Finalidade.Receita)] // Categoria incompatível
            public async Task Deve_Lancar_Excecao_E_Nao_Persistir_Quando_Regra_De_Dominio_Falhar(
                decimal valor, TipoTransacao tipo, int idade, Finalidade finalidadeCat)
            {
                var pessoa = new Pessoa("Teste", DateTime.Today.AddYears(-idade));
                var categoria = new Categoria("Categoria", finalidadeCat);
                var dto = new CreateTransacaoDto
                {
                    PessoaId = Guid.NewGuid(),
                    CategoriaId = Guid.NewGuid(),
                    Valor = valor,
                    Tipo = tipo,
                    Data = DateTime.Today,
                    Descricao = "Teste"
                };

                _pessoaRepositoryMock.Setup(r => r.GetByIdAsync(dto.PessoaId)).ReturnsAsync(pessoa);
                _categoriaRepositoryMock.Setup(r => r.GetByIdAsync(dto.CategoriaId)).ReturnsAsync(categoria);

                Func<Task> acao = async () => await _service.CreateAsync(dto);

                await acao.Should().ThrowAsync<Exception>();
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Transacao>()), Times.Never);
                _uowMock.Verify(u => u.CommitAsync(), Times.Never);
            }
        }
    }
}
