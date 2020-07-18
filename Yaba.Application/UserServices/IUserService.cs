using System.Threading.Tasks;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.UserServices
{
    public interface IUserService
    {
        Task UserSignIn(UserSignInDTO dto);
    }
}
