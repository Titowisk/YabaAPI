using System.Threading.Tasks;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Security;
using Yaba.Tools.Validations;

namespace Yaba.Application.UserServices.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task UserSignIn(UserSignInDTO dto)
        {
            ValidateUser(dto.Name, dto.Email, dto.Password);

            // encrypt password
            dto.Password = EncryptPassword(dto.Password);

            var user = new User(dto.Name, dto.Email, dto.Password);
            await _userRepository.Create(user);
        }

        #region Priv Methods
        private string EncryptPassword(string password)
        {
            return SecurityManager.GeneratePbkdf2Hash(password);
        }

        private async Task ValidateUser(string name, string email, string password)
        {
            Validate.NotNullOrEmpty(name, "A name is necessary");

            await ValidateEmail(email);

            ValidatePassword(password);
        }

        private async Task ValidateEmail(string email)
        {
            // TODO: proper email validation
            Validate.NotNullOrEmpty(email, "An email is necessary");

            Validate.IsTrue(email.Contains("@"), "Email is not valid");

            var emailExists = await _userRepository.UserWithEmailExists(email);

            Validate.IsTrue(!emailExists, "This email has already been signed in");
        }

        private void ValidatePassword(string password)
        {
            // TODO: proper password validation
            Validate.NotNullOrEmpty(password, "A password is necessary");

            Validate.IsTrue(password.Length > 7, "Password needs to be 8 characters long or greater");
        }
        #endregion
    }
}
