using AutoMapper;
using Bogus;
using ControleGastos.Application.DTOs.Pessoa;
using ControleGastos.Application.Services;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;
using FluentAssertions;
using Moq;

namespace UnitTestes.ServicesTestes
{
    public class PessoaServiceTestes
    {
        private readonly Mock<IPessoaRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PessoaService _service;
        private readonly Faker _faker;

        public PessoaServiceTestes()
        {
            _repositoryMock = new Mock<IPessoaRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _faker = new Faker("pt_BR");

            _service = new PessoaService(
                _repositoryMock.Object,
                _uowMock.Object,
                _mapperMock.Object);
        }

        public class CriarAsync : PessoaServiceTestes
        {
            [Fact]
            public async Task Deve_Criar_Pessoa_Com_Sucesso()
            {
                var dto = new CreatePessoaDto
                {
                    Nome = _faker.Person.FullName,
                    DataNascimento = _faker.Date.Past(20)
                };

                var pessoaDto = new PessoaDto { Nome = dto.Nome };

                _mapperMock.Setup(m => m.Map<PessoaDto>(It.IsAny<Pessoa>())).Returns(pessoaDto);

                var resultado = await _service.CreateAsync(dto);

                resultado.Should().NotBeNull();
                _repositoryMock.Verify(r => r.AddAsync(It.Is<Pessoa>(p => p.Nome == dto.Nome)), Times.Once);
                _uowMock.Verify(u => u.CommitAsync(), Times.Once);
            }

            [Theory]
            [InlineData("", 0)]
            [InlineData("Teste", 1)]
            public async Task Deve_Lancar_Excecao_E_Nao_Persistir_Quando_Dados_Invalidos(string nome, int diasParaAdicionar)
            {
                var dto = new CreatePessoaDto
                {
                    Nome = nome,
                    DataNascimento = DateTime.Today.AddDays(diasParaAdicionar)
                };

                Func<Task> acao = async () => await _service.CreateAsync(dto);

                await acao.Should().ThrowAsync<ArgumentException>();
                _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Pessoa>()), Times.Never);
                _uowMock.Verify(u => u.CommitAsync(), Times.Never);
            }
        }

        public class AtualizarAsync : PessoaServiceTestes
        {
            [Fact]
            public async Task Deve_Atualizar_Pessoa_Com_Sucesso()
            {
                var id = Guid.NewGuid();
                var pessoaExistente = new Pessoa("Nome Antigo", DateTime.Today.AddYears(-20));
                var dto = new UpdatePessoaDto
                {
                    Nome = _faker.Person.FullName,
                    DataNascimento = _faker.Date.Past(25)
                };

                _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(pessoaExistente);

                await _service.UpdateAsync(id, dto);

                pessoaExistente.Nome.Should().Be(dto.Nome);
                _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Pessoa>()), Times.Once);
                _uowMock.Verify(u => u.CommitAsync(), Times.Once);
            }

            [Fact]
            public async Task Deve_Lancar_Excecao_Quando_Pessoa_Nao_Encontrada()
            {
                var id = Guid.NewGuid();
                var dto = new UpdatePessoaDto { Nome = "Teste" };

                _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Pessoa)null!);

                Func<Task> acao = async () => await _service.UpdateAsync(id, dto);

                await acao.Should().ThrowAsync<Exception>().WithMessage("Pessoa não encontrada.");
                _uowMock.Verify(u => u.CommitAsync(), Times.Never);
            }
        }
    }
}
