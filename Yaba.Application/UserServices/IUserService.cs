using System.Threading.Tasks;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.UserServices
{
    public interface IUserService
    {
        Task UserSignUp(UserSignUpDTO dto);
        Task<UserLoginResponseDTO> Login(UserLoginDTO dto);
        Task<UserLoginResponseDTO> GetCurrentUserById(int id);
    }
}
