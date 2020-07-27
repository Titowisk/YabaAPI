using System;
using System.Threading.Tasks;
using Yaba.Infrastructure.Persistence.Context;

namespace Yaba.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IDisposable
    {
        private readonly DataContext _context;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        public bool Commit()
        {
            var rowsCommitted = _context.SaveChanges();
            return rowsCommitted > 0;
        }

        public async Task<bool> CommitAsync()
        {
            var rowsCommitted = await _context.SaveChangesAsync();

            return rowsCommitted > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
