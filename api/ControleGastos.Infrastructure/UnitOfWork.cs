using ControleGastos.Domain.Interfaces;

namespace ControleGastos.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ControleGastosDbContext _context;

        public UnitOfWork(ControleGastosDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CommitAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
