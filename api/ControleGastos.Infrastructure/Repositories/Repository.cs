using ControleGastos.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Repositories
{
    // Classe base abstrata que implementa o repositório genérico, fornecendo a lógica padrão de acesso a dados via Entity Framework.
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ControleGastosDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected Repository(ControleGastosDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Recupera todos os registros da entidade; o parâmetro 'search' pode ser implementado via override nos repositórios específicos.
        public virtual async Task<IEnumerable<T>> GetAllAsync(string? search = null)
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        // Busca um registro único pelo seu identificador (Guid) no banco de dados.
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        // Prepara a entidade para ser inserida no contexto do banco de dados.
        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // Marca a entidade como modificada para que o EF atualize seus dados no banco.
        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        // Localiza a entidade pelo ID e a remove do contexto para exclusão física no banco.
        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) _dbSet.Remove(entity);
        }
    }
}
