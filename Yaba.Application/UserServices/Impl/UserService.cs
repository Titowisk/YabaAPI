﻿using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Persistence.UnitOfWork;
using Yaba.Infrastructure.Security;
using Yaba.Tools.Validations;

namespace Yaba.Application.UserServices.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOptions<JwtConfig> _options;
        private readonly UnitOfWork _uow;

        public UserService(
            IUserRepository userRepository,
            UnitOfWork uow,
            IOptions<JwtConfig> options)
        {
            _userRepository = userRepository;
            _options = options;
            this._uow = uow;
        }

        public async Task<UserLoginResponseDTO> Login(UserLoginDTO dto)
        {
            var user = await _userRepository.GetByEmail(dto.Email);

            Validate.NotNull(user, "User not found");

            var passwordIsValid = SecurityManager.VerifyPasswordPbkdf2(dto.Password, user.Password);

            Validate.IsTrue(passwordIsValid, "Password is incorrect");

            return new UserLoginResponseDTO()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = JwtHandler.GenerateToken(_options.Value.SecretKey, user.Id, user.Name)
            };
        }

        public async Task UserSignUp(UserSignUpDTO dto)
        {
            // TODO: add email confirmation
            await ValidateUser(dto.Name, dto.Email, dto.Password);

            dto.Password = EncryptPassword(dto.Password);

            var user = new User(dto.Name, dto.Email, dto.Password);
            _userRepository.Insert(user);
            await _uow.CommitAsync();
        }

        public async Task<UserLoginResponseDTO> GetCurrentUserById(int id)
        {
            var user = await _userRepository.GetById(id);
            Validate.NotNull(user, "User not found");

            return new UserLoginResponseDTO { Id = user.Id, Name = user.Name, Email = user.Email };
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
