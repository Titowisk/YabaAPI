using System;
using System.Threading.Tasks;

namespace Yaba.Domain.Models.Users
{
    public interface IUserRepository : IDisposable
    {
        Task<bool> UserWithEmailExists(string email);

        Task<User> GetByEmail(string email);
        Task<User> GetById(int id);

        void Update(User entity);

        void Delete(User entity);

        void Insert(User entity);
    }
}
