using ControleGastos.Domain.DTOs;

namespace ControleGastos.Domain.Interfaces
{
    public interface ITotaisRepository
    {
        Task<RelatorioPessoasGeralDto> GetRelatorioPessoasCompletoAsync();
        Task<RelatorioCategoriaGeralDto> GetRelatorioCategoriasCompletoAsync();
    }
}
