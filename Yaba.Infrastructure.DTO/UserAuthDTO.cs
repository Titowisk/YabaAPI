namespace Yaba.Infrastructure.DTO
{
    public class UserSignInDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginResponseDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
