using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yaba.Domain.Models.Transactions;
using Yaba.Infrastructure.DTO;
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

        public async Task<bool> DoesTransactionExists(string hash) 
        {
            return await _context.Transactions.AnyAsync(t => t.Metadata == hash);
        }


        public void Create(Transaction entity)
        {
            _context.Transactions.Add(entity);
        }

        public async Task<IEnumerable<DateDTO>> GetDatesByUser(int userId, int bankAccountId)
        {
            var query = _context.Transactions
                         .Where(t => t.BankAccountId == bankAccountId)
                         .Where(t => t.BankAccount.UserId == userId)
                         .Select(t => new DateDTO() { Year = t.Date.Year, Month = t.Date.Month })
                         .OrderByDescending(t => t.Year).ThenBy(t => t.Month);

            return await query.ToListAsync();
        }


        public async Task<Transaction> GetByIdWithBankAccount(long id)
        {
            var transaction = await _context
                    .Transactions
                    .Include(t => t.BankAccount)
                    .FirstOrDefaultAsync(t => t.Id == id);

            return transaction;
        }

        public async Task<ICollection<TransactionsDateFilterResponseDTO>> GetByMonthBankAccountUser(short year, short month, int bankAccountId, int userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.Date.Year == year)
                .Where(t => t.Date.Month == month)
                .Include(t => t.BankAccount)
                .Where(t => t.BankAccountId == bankAccountId)
                .Where(t => t.BankAccount.UserId == userId)
                .Select(t => new TransactionsDateFilterResponseDTO() 
                {
                    Id = t.Id,
                    Amount = t.Amount, 
                    Date = t.Date, 
                    Origin = t.Origin,
                    Category = t.Category.ToString(),
                    CategoryId = (short?)t.Category 
                })
                .OrderBy(t => t.Date)
                .ToListAsync();

            return transactions;
        }

        public async Task<IEnumerable<Transaction>> GetByDateAndOrigin(DateTime date, string origin, int bankAccountId)
        {
            var query = _context.Transactions
                .Where(t => t.Date.Year == date.Year)
                .Where(t => t.Date.Month == date.Month)
                .Where(t => t.BankAccountId == bankAccountId)
                .Where((t) => t.Origin.Equals(origin));
                
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllOtherTransactions(Transaction recentlyUpdatedtransaction)
        {
            var query = _context.Transactions
                .Where(t => t.BankAccountId == recentlyUpdatedtransaction.BankAccountId)
                .Where(t => t.Category.Value != recentlyUpdatedtransaction.Category)
                .Where((t) => t.Origin.Equals(recentlyUpdatedtransaction.Origin));

            return await query.ToListAsync();
        }
    }
}
