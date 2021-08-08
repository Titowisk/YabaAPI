using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.Persistence.Abstracts;
using Yaba.Infrastructure.Persistence.Context;

namespace Yaba.Infrastructure.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context): base(context)
        {
            _context = context;
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public async Task<bool> UserWithEmailExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.Equals(email));
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }
  
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
