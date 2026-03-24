using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Repositories
{
    public class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
    {
        public TransacaoRepository(ControleGastosDbContext context) : base(context) { }

        // Recupera uma transação específica pelo ID, carregando os dados de Pessoa e Categoria vinculados.
        public override async Task<Transacao?> GetByIdAsync(Guid id)
        {
            return await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // Recupera todas as transações cadastradas, incluindo as informações de Pessoa e Categoria para cada registro.
        public override async Task<IEnumerable<Transacao>> GetAllAsync(string? search = null)
        {
            return await _context.Transacoes
                .Include(t => t.Pessoa)
                .Include(t => t.Categoria)
                .AsNoTracking()
                .ToListAsync();
        }

        // Verifica se existe ao menos uma transação vinculada a uma categoria específica.
        public async Task<bool> ExisteTransacaoComCategoriaAsync(Guid categoriaId)
        {
            return await _context.Transacoes.AnyAsync(t => t.CategoriaId == categoriaId);
        }
    }
}
