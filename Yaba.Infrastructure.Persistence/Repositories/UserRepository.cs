using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.Persistence.Context;

namespace Yaba.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> UserWithEmailExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.Equals(email));
        }

        public async Task Create(User entity)
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var entity = await _context.Users.FindAsync(id);
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(User entity)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Update(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
