namespace ControleGastos.Domain.Interfaces
{
    // Interface que gerencia a persistência de dados, garantindo que todas as operações sejam salvas de uma única vez no banco.
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> CommitAsync();
    }
}
