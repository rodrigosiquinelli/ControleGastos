using ControleGastos.Application.Services;
using ControleGastos.Domain.DTOs;
using ControleGastos.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace UnitTestes.ServicesTestes
{
    public class TotaisServiceTestes
    {
        private readonly Mock<ITotaisRepository> _totaisRepositoryMock;
        private readonly TotaisService _service;

        public TotaisServiceTestes()
        {
            _totaisRepositoryMock = new Mock<ITotaisRepository>();
            _service = new TotaisService(_totaisRepositoryMock.Object);
        }

        public class GetRelatorioPessoasAsync : TotaisServiceTestes
        {
            [Fact]
            public async Task Deve_Retornar_Relatorio_Pessoas_Com_Sucesso()
            {
                var relatorioEsperado = new RelatorioPessoasGeralDto();
                _totaisRepositoryMock.Setup(r => r.GetRelatorioPessoasCompletoAsync())
                    .ReturnsAsync(relatorioEsperado);

                var resultado = await _service.GetRelatorioPessoasAsync();

                resultado.Should().BeEquivalentTo(relatorioEsperado);
                _totaisRepositoryMock.Verify(r => r.GetRelatorioPessoasCompletoAsync(), Times.Once);
            }
        }

        public class GetRelatorioCategoriasAsync : TotaisServiceTestes
        {
            [Fact]
            public async Task Deve_Retornar_Relatorio_Categorias_Com_Sucesso()
            {
                var relatorioEsperado = new RelatorioCategoriaGeralDto();
                _totaisRepositoryMock.Setup(r => r.GetRelatorioCategoriasCompletoAsync())
                    .ReturnsAsync(relatorioEsperado);

                var resultado = await _service.GetRelatorioCategoriasAsync();

                resultado.Should().BeEquivalentTo(relatorioEsperado);
                _totaisRepositoryMock.Verify(r => r.GetRelatorioCategoriasCompletoAsync(), Times.Once);
            }
        }
    }
}
