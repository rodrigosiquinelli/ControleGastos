namespace ControleGastos.Domain.Interfaces
{
    /* Interface genérica que define o contrato padrão para operações de persistência (CRUD), 
     permitindo a reutilização da mesma base de acesso a dados para diferentes entidades. */
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(string? search = null);
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}
