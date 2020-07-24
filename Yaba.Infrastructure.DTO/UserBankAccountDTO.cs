namespace Yaba.Infrastructure.DTO
{
    public class CreateUserBankAccountDTO
    {
        public int UserId { get; set; }
        public string Number { get; set; }
        public string Agency { get; set; }
        public short Code { get; set; }
    }

    public class UserBankAccountDTO 
    {
        public int UserId { get; set; }
        public string Number { get; set; }
        public string Agency { get; set; }
        public short Code { get; set; }
    }
}
