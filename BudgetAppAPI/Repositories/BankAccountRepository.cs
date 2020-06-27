using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YabaAPI.Models;
using YabaAPI.Repositories.Contracts;

namespace YabaAPI.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly DataContext _context;

        public BankAccountRepository(DataContext context)
        {
            _context = context;
        }

        public async Task Create(BankAccount entity)
        {
            if (entity.Transactions.Count > 0)
                _context.Attach(entity);

            _context.BankAccounts.Add(entity);

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _context.BankAccounts.FindAsync(id);
            _context.BankAccounts.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.BankAccounts.AnyAsync(e => e.Id == id);
        }

        public BankAccount? Find(BankAccount bankAccount)
        {
            return _context.BankAccounts
                        .FirstOrDefault(bk =>
                            bk.Number == bankAccount.Number &&
                            bk.Agency == bankAccount.Agency &&
                            bk.Code == bankAccount.Code);
        }

        public async Task<IEnumerable<BankAccount>> GetAll()
        {
            return await _context
                    .BankAccounts
                    .ToListAsync();
        }

        public async Task<BankAccount> GetById(int id)
        {
            return await _context
                .BankAccounts
                .Include(b => b.Transactions)
                .FirstAsync(b => b.Id == id);
        }

        public async Task Update(BankAccount entity)
        {
            _context.BankAccounts.Update(entity);

            await _context.SaveChangesAsync();
        }
    }
}
