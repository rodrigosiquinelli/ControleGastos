using AutoMapper;
using ControleGastos.Application.DTOs.Pessoa;
using ControleGastos.Application.Services;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;
using FluentAssertions;
using Moq;

namespace UnitTestes.ServicesTestes
{
    public class BaseServiceTestes
    {
        private readonly Mock<IRepository<Pessoa>> _repositoryMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TesteBaseService _service;

        public BaseServiceTestes()
        {
            _repositoryMock = new Mock<IRepository<Pessoa>>();
            _uowMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _service = new TesteBaseService(
                _repositoryMock.Object,
                _uowMock.Object,
                _mapperMock.Object);
        }

        // Testando apenas a lógica do BaseService
        public class TesteBaseService : BaseService<Pessoa, PessoaDto, CreatePessoaDto, UpdatePessoaDto>
        {
            public TesteBaseService(IRepository<Pessoa> repo, IUnitOfWork uow, IMapper mapper)
                : base(repo, uow, mapper) { }

            public override Task<PessoaDto> CreateAsync(CreatePessoaDto dto) => throw new NotImplementedException();
            public override Task UpdateAsync(Guid id, UpdatePessoaDto dto) => throw new NotImplementedException();
        }

        public class GetAllAsync : BaseServiceTestes
        {
            [Fact]
            public async Task Deve_Retornar_Lista_De_Dtos_Mapeados()
            {
                var entities = new List<Pessoa> { new Pessoa("Teste", DateTime.Today.AddYears(-20)) };
                var dtos = new List<PessoaDto> { new PessoaDto { Nome = "Teste" } };

                _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<string>())).ReturnsAsync(entities);
                _mapperMock.Setup(m => m.Map<IEnumerable<PessoaDto>>(entities)).Returns(dtos);

                var resultado = await _service.GetAllAsync("busca");

                resultado.Should().HaveCount(1);
                _repositoryMock.Verify(r => r.GetAllAsync("busca"), Times.Once);
            }
        }

        public class GetByIdAsync : BaseServiceTestes
        {
            [Fact]
            public async Task Deve_Retornar_Dto_Quando_Entidade_Existe()
            {
                var id = Guid.NewGuid();
                var entidade = new Pessoa("Teste", DateTime.Today.AddYears(-20));
                var dto = new PessoaDto { Nome = "Teste" };

                _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entidade);
                _mapperMock.Setup(m => m.Map<PessoaDto>(entidade)).Returns(dto);

                var resultado = await _service.GetByIdAsync(id);

                resultado.Should().NotBeNull();
                _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            }

            [Fact]
            public async Task Deve_Retornar_Nulo_Quando_Entidade_Nao_Existe()
            {
                var id = Guid.NewGuid();
                _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Pessoa)null!);

                var resultado = await _service.GetByIdAsync(id);

                resultado.Should().BeNull();
            }
        }

        public class DeleteAsync : BaseServiceTestes
        {
            [Fact]
            public async Task Deve_Chamar_Delete_E_Commit_Com_Sucesso()
            {
                var id = Guid.NewGuid();

                await _service.DeleteAsync(id);

                _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
                _uowMock.Verify(u => u.CommitAsync(), Times.Once);
            }
        }
    }
}
