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

        public async Task<ICollection<TransactionsResponseDTO>> GetByMonthBankAccountUser(short year, short month, int bankAccountId, int userId)
        {
            var transactions = _context.Transactions
                .Where(t => t.Date.Year == year)
                .Where(t => t.Date.Month == month)
                .Include(t => t.BankAccount)
                .Where(t => t.BankAccountId == bankAccountId)
                .Where(t => t.BankAccount.UserId == userId)
                .Select(t => new TransactionsResponseDTO()
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Date = t.Date,
                    Origin = t.Origin,
                    Category = t.Category.ToString(),
                    CategoryId = (short?)t.Category
                })
                .OrderBy(t => t.Date)
                ;

            return await transactions.ToListAsync();
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

        public async Task<IEnumerable<Transaction>> GetByDateAndOrigin(int userId, int bankAccountId, string origin, int? year, int? month)
        {
            var query = _context.Transactions
                //.Include(t => t.BankAccount)
                .Where(t => t.BankAccount.UserId == userId)
                .Where(t => t.BankAccountId == bankAccountId)
                .Where((t) => t.Origin.Equals(origin));

            if (year.HasValue)
            {
                query = query.Where(t => t.Date.Year == year.Value);
                if (month.HasValue) query = query.Where(t => t.Date.Month == month.Value);
            }


            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllOtherTransactionsOutsideMonth(Transaction recentlyUpdatedtransaction)
        {
            var query = _context.Transactions
                .Where(t => t.BankAccountId == recentlyUpdatedtransaction.BankAccountId)
                .Where(t => t.Category.Value != recentlyUpdatedtransaction.Category)
                .Where((t) => t.Origin.Equals(recentlyUpdatedtransaction.Origin))
                .Where(t => t.Date.Year != recentlyUpdatedtransaction.Date.Year && t.Date.Month != recentlyUpdatedtransaction.Date.Month)
                ;
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

        public async Task<IEnumerable<Transaction>> GetPredecessors(DateTime referenceDate, int bankAccountId)
        {
            // TODO: months with 30/31 days or frebuary can break it?
            var query = _context.Transactions
                .Where(t => t.BankAccountId == bankAccountId)
                .Where(t => t.Date > referenceDate.AddMonths(-6) && t.Date < referenceDate)
                .Where(t => t.Category != null)
                ;

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Transaction>> GetByIds(long[] ids)
        {
            var query = _context.Transactions
                .Where(t => ids.Contains(t.Id))
                ;

            return await query.ToListAsync();
        }


    }
}
