using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.DTOs;
using ControleGastos.Domain.Interfaces;

namespace ControleGastos.Application.Services
{
    // Serviço especializado em consolidar dados e gerar relatórios financeiros agregados.
    public class TotaisService : ITotaisService
    {
        private readonly ITotaisRepository _totaisRepository;

        public TotaisService(ITotaisRepository totaisRepository)
        {
            _totaisRepository = totaisRepository;
        }

        // Recupera o relatório detalhado de gastos e receitas agrupados por pessoa.
        public async Task<RelatorioPessoasGeralDto> GetRelatorioPessoasAsync()
        {
            return await _totaisRepository.GetRelatorioPessoasCompletoAsync();
        }

        // Recupera o relatório detalhado de movimentações financeiras separadas por categorias.
        public async Task<RelatorioCategoriaGeralDto> GetRelatorioCategoriasAsync()
        {
            return await _totaisRepository.GetRelatorioCategoriasCompletoAsync();
        }
    }
}
