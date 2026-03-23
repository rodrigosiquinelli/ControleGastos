using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.DTOs;
using ControleGastos.Domain.Interfaces;

namespace ControleGastos.Application.Services
{
    public class TotaisService : ITotaisService
    {
        private readonly ITotaisRepository _totaisRepository;

        public TotaisService(ITotaisRepository totaisRepository)
        {
            _totaisRepository = totaisRepository;
        }

        public async Task<RelatorioPessoasGeralDto> GetRelatorioPessoasAsync()
        {
            return await _totaisRepository.GetRelatorioPessoasCompletoAsync();
        }

        public async Task<RelatorioCategoriaGeralDto> GetRelatorioCategoriasAsync()
        {
            return await _totaisRepository.GetRelatorioCategoriasCompletoAsync();
        }
    }
}
