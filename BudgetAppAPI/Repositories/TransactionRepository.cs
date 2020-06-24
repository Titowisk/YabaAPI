using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using YabaAPI.Models;
using YabaAPI.Repositories.Contracts;

namespace YabaAPI.Repositories
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
        }

        public void Delete(long id)
        {
            var transaction = _context.Transactions.Find(id);
            _context.Transactions.Remove(transaction);
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
                    .First(t => t.Id == id);

            return transaction;
        }

        public Transaction GetByIdWithBankAccount(long id)
        {
            var transaction = _context
                    .Transactions
                    .Include(t => t.BankAccount)
                    .First(t => t.Id == id);

            return transaction;
        }

        public void Update(Transaction entity)
        {
            _context.Transactions.Update(entity);
        }

        public void Save()
        {
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
    }
    /*NOTES
     - https://docs.microsoft.com/pt-br/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
    */
}
