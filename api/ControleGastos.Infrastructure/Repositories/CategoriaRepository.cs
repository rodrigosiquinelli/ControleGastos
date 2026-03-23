using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ControleGastosDbContext context) : base(context) { }

        public async Task<IEnumerable<Categoria>> GetCategoriasComTransacoesAsync()
        {
            return await _context.Categorias
                .Include(c => c.Transacoes)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
