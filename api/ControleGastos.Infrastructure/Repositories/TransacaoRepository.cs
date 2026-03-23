using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ControleGastos.Infrastructure.Repositories
{
    public class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
    {
        public TransacaoRepository(ControleGastosDbContext context) : base(context) { }

        // Sobrescrevemos o GetAll para já trazer os nomes de Pessoa e Categoria (Eager Loading)
        public override async Task<Transacao?> GetByIdAsync(Guid id)
        {
            return await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // Sobrescrevemos o GetAll para as listagens da API virem completas
        public override async Task<IEnumerable<Transacao>> GetAllAsync(string? search = null)
        {
            return await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExisteTransacaoComCategoriaAsync(Guid categoriaId)
        {
            return await _context.Transacoes.AnyAsync(t => t.CategoriaId == categoriaId);
        }
    }
}
