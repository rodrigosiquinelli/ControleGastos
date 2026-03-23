using ControleGastos.Domain.Models;

namespace ControleGastos.Domain.Interfaces
{
    public interface IPessoaRepository : IRepository<Pessoa>
    {
        Task<IEnumerable<Pessoa>> GetPessoasComTransacoesAsync();
    }
}
