using ControleGastos.Domain.Models;
using System.Linq.Expressions;

namespace ControleGastos.Domain.Interfaces
{
    public interface ITransacaoRepository : IRepository<Transacao>
    {
        Task<bool> ExisteTransacaoComCategoriaAsync(Guid categoriaId);
    }
}
