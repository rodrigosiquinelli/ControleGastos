using ControleGastos.Domain.DTOs;

namespace ControleGastos.Application.Interfaces
{
    public interface ITotaisService
    {
        Task<RelatorioPessoasGeralDto> GetRelatorioPessoasAsync();
        Task<RelatorioCategoriaGeralDto> GetRelatorioCategoriasAsync();
    }
}
