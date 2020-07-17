using System;
using System.Threading.Tasks;

namespace Yaba.Domain.Models.Users
{
    public interface IUserRepository : IAsyncDisposable
    {
        Task<User> GetById(int id);
        Task Create(User entity);
        Task Update(User entity);
        Task Delete(int id);
        Task Delete(User entity);
    }
}
