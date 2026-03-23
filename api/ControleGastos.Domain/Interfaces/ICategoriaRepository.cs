using ControleGastos.Domain.Models;

namespace ControleGastos.Domain.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> GetCategoriasComTransacoesAsync();
    }
}
