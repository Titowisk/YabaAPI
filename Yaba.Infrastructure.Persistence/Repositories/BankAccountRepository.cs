using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaba.Domain.Models;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Infrastructure.Persistence.Abstracts;
using Yaba.Infrastructure.Persistence.Context;

namespace Yaba.Infrastructure.Persistence.Repositories
{
    public class BankAccountRepository : GenericRepository<BankAccount>, IBankAccountRepository
    {
        private readonly DataContext _context;

        public BankAccountRepository(DataContext context) :
            base(context)
        {
            _context = context;
        }

        public async Task<BankAccount> GetBy(string agency, string number, short code)
        {
            return await _context.BankAccounts
                        .Include(bk => bk.User)
                        .FirstOrDefaultAsync(bk =>
                            bk.Number == number &&
                            bk.Agency == agency &&
                            bk.Code == code);
        }

        public override async Task<BankAccount> GetById(object id)
        {
            return await _context
                .BankAccounts
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == (int)id);
        }
    }
}
