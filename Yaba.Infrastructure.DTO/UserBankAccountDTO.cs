﻿namespace Yaba.Infrastructure.DTO
{
    public class CreateUserBankAccountDTO
    {
        public int UserId { get; set; }
        public string Number { get; set; }
        public string Agency { get; set; }
        public short Code { get; set; }
    }

    public class GetUserBankAccountDTO 
    {
        public int UserId { get; set; }
        public int BankAccountId { get; set; }
        public string Number { get; set; }
        public string Agency { get; set; }
        public short Code { get; set; }
    }

    public class GetUserBankAccountsDTO
    {
        public int UserId { get; set; }

    }

    public class UpdateUserBankAccountDTO
    {
        public int UserId { get; set; }
        public int BankAccountId { get; set; }
        public string Number { get; set; }
        public string Agency { get; set; }
        public short Code { get; set; }
    }

    public class DeleteUserBankAccountDTO
    {
        public int UserId { get; set; }
        public int BankAccountId { get; set; }
    }
}
