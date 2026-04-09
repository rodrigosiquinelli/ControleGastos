using AutoMapper;
using Bogus;
using ControleGastos.Application.DTOs.Categoria;
using ControleGastos.Application.Services;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;
using FluentAssertions;
using Moq;

namespace UnitTestes.ServicesTestes
{
    public class CategoriaServiceTestes
    {
        private readonly Mock<ICategoriaRepository> _repositoryMock;
        private readonly Mock<ITransacaoRepository> _transacaoRepositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CategoriaService _service;
        private readonly Faker _faker;

        public CategoriaServiceTestes()
        {
            _repositoryMock = new Mock<ICategoriaRepository>();
            _transacaoRepositoryMock = new Mock<ITransacaoRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _faker = new Faker("pt_BR");

            _service = new CategoriaService(
                _repositoryMock.Object,
                _transacaoRepositoryMock.Object,
                _uowMock.Object,
                _mapperMock.Object);
        }

        public class CriarAsync : CategoriaServiceTestes
        {
            [Fact]
            public async Task Deve_Criar_Categoria_Com_Sucesso()
            {
                var dto = new CreateCategoriaDto
                {
                    Descricao = _faker.Commerce.Categories(1)[0],
                    Finalidade = Finalidade.Ambas
                };

                _mapperMock.Setup(m => m.Map<CategoriaDto>(It.IsAny<Categoria>())).Returns(new CategoriaDto());

                var resultado = await _service.CreateAsync(dto);

                resultado.Should().NotBeNull();
                _repositoryMock.Verify(r => r.AddAsync(It.Is<Categoria>(c => c.Descricao == dto.Descricao)), Times.Once);
                _uowMock.Verify(u => u.CommitAsync(), Times.Once);
            }

            [Theory]
            [InlineData("")]
            [InlineData(" ")]
            public async Task Deve_Lancar_Excecao_E_Nao_Persistir_Quando_Dados_Invalidos(string descricao)
            {
                var dto = new CreateCategoriaDto { Descricao = descricao, Finalidade = Finalidade.Despesa };

                Func<Task> acao = async () => await _service.CreateAsync(dto);

                await acao.Should().ThrowAsync<ArgumentException>();
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Never);
                _uowMock.Verify(u => u.CommitAsync(), Times.Never);
            }
        }

        public class AtualizarAsync : CategoriaServiceTestes
        {
            [Fact]
            public async Task Deve_Atualizar_Categoria_Com_Sucesso_Quando_Nao_Ha_Mudanca_De_Finalidade()
            {
                var id = Guid.NewGuid();
                var categoriaExistente = new Categoria("Antiga", Finalidade.Despesa);
                var dto = new CategoriaDto { Descricao = "Nova", Finalidade = Finalidade.Despesa };

                _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(categoriaExistente);

                await _service.UpdateAsync(id, dto);

                categoriaExistente.Descricao.Should().Be("Nova");
                _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Categoria>()), Times.Once);
                _uowMock.Verify(u => u.CommitAsync(), Times.Once);
            }

            [Fact]
            public async Task Deve_Atualizar_Categoria_Com_Sucesso_Quando_Finalidade_Muda_E_Nao_Ha_Transacoes()
            {
                var id = Guid.NewGuid();
                var categoriaExistente = new Categoria("Teste", Finalidade.Despesa);
                var dto = new CategoriaDto { Descricao = "Teste", Finalidade = Finalidade.Receita };

                _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(categoriaExistente);
                _transacaoRepositoryMock.Setup(r => r.ExisteTransacaoComCategoriaAsync(id)).ReturnsAsync(false);

                await _service.UpdateAsync(id, dto);

                categoriaExistente.Finalidade.Should().Be(Finalidade.Receita);
                _uowMock.Verify(u => u.CommitAsync(), Times.Once);
            }

            [Fact]
            public async Task Deve_Lancar_Excecao_Quando_Finalidade_Muda_E_Existem_Transacoes()
            {
                var id = Guid.NewGuid();
                var categoriaExistente = new Categoria("Teste", Finalidade.Despesa);
                var dto = new CategoriaDto { Descricao = "Teste", Finalidade = Finalidade.Receita };

                _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(categoriaExistente);
                _transacaoRepositoryMock.Setup(r => r.ExisteTransacaoComCategoriaAsync(id)).ReturnsAsync(true);

                Func<Task> acao = async () => await _service.UpdateAsync(id, dto);

                await acao.Should().ThrowAsync<InvalidOperationException>()
                    .WithMessage("Não é possível alterar a finalidade de categorias que possuem transações vinculadas.");
                _uowMock.Verify(u => u.CommitAsync(), Times.Never);
            }

            [Fact]
            public async Task Deve_Lancar_Excecao_Quando_Categoria_Nao_Encontrada()
            {
                var id = Guid.NewGuid();
                _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Categoria)null!);

                Func<Task> acao = async () => await _service.UpdateAsync(id, new CategoriaDto());

                await acao.Should().ThrowAsync<Exception>().WithMessage("Categoria não encontrada.");
            }
        }
    }
}
