using ControleGastos.Domain.Models;

namespace ControleGastos.Domain.Interfaces
{
    public interface ITransacaoRepository : IRepository<Transacao>
    {
        Task<bool> ExisteTransacaoComCategoriaAsync(Guid categoriaId);
    }
}
