using ControleGastos.Domain.Interfaces;

namespace ControleGastos.Infrastructure
{
    // Implementa o padrão Unit of Work, garantindo que todas as operações realizadas no contexto sejam persistidas como uma única unidade.
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ControleGastosDbContext _context;

        public UnitOfWork(ControleGastosDbContext context)
        {
            _context = context;
        }

        // Confirma as alterações no banco de dados; retorna verdadeiro se ao menos uma linha foi afetada.
        public async Task<bool> CommitAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // Libera os recursos do contexto de banco de dados e solicita que o Garbage Collector não execute o finalizador.
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
