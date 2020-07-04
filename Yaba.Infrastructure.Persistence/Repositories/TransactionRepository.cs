using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Yaba.Domain.Models.Transactions;
using Yaba.Infrastructure.Persistence.Context;

namespace Yaba.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository, IDisposable
    {
        private readonly DataContext _context;
        private bool _disposed = false;

        public TransactionRepository(DataContext context)
        {
            _context = context;
        }

        public void Create(Transaction entity)
        {
            _context.Transactions.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(long id)
        {
            var transaction = _context.Transactions.Find(id);
            _context.Transactions.Remove(transaction);

            _context.SaveChanges();
        }

        public void Delete(Transaction entity)
        {
            _context.Transactions.Remove(entity);

            _context.SaveChanges();
        }

        public IEnumerable<Transaction> GetAll()
        {
            var transactions = _context
                    .Transactions
                    .Include(t => t.BankAccount)
                    .ToList();

            return transactions;
        }

        public Transaction GetById(long id)
        {
            var transaction = _context
                    .Transactions
                    .FirstOrDefault(t => t.Id == id);

            return transaction;
        }

        public Transaction GetByIdWithBankAccount(long id)
        {
            var transaction = _context
                    .Transactions
                    .Include(t => t.BankAccount)
                    .FirstOrDefault(t => t.Id == id);

            return transaction;
        }

        public void Update(Transaction entity)
        {
            _context.Transactions.Update(entity);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this._disposed = true;
        }
        /*NOTES
            - https://docs.microsoft.com/pt-br/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
        */
    }
}
