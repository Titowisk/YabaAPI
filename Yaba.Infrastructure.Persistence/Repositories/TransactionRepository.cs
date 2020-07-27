using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yaba.Domain.Models.Transactions;
using Yaba.Infrastructure.Persistence.Abstracts;
using Yaba.Infrastructure.Persistence.Context;

namespace Yaba.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly DataContext _context;

        public TransactionRepository(DataContext context)
            : base(context)
        {
            _context = context;
        }

        public void Create(Transaction entity)
        {
            _context.Transactions.Add(entity);
        }

        public async Task<Transaction> GetByIdWithBankAccount(long id)
        {
            var transaction = await _context
                    .Transactions
                    .Include(t => t.BankAccount)
                    .FirstOrDefaultAsync(t => t.Id == id);

            return transaction;
        }
    }
}
