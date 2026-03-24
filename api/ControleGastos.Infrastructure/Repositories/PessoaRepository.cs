using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Repositories
{
    public class PessoaRepository : Repository<Pessoa>, IPessoaRepository
    {
        public PessoaRepository(ControleGastosDbContext context) : base(context) { }

        // Recupera todas as pessoas e carrega suas transações vinculadas, se houverem.
        public async Task<IEnumerable<Pessoa>> GetPessoasComTransacoesAsync()
        {
            return await _context.Pessoas
                .Include(p => p.Transacoes)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
