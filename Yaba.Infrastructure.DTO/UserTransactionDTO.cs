using System;

namespace Yaba.Infrastructure.DTO
{
    public class DeleteUserTransactionDTO
    {
        public int UserId { get; set; }
        public int BankAccountId { get; set; }
        public long TransactionId { get; set; }
    }

    public class CategorizeUserTransactionsDTO
    {
        public long TransactionId { get; set; }
        public int UserId { get; set; }
        public short CategoryId { get; set; }
    }

    public class CreateUserTransactionDTO
    {
        public int UserId { get; set; }
        public int BankAccountId { get; set; }
        public string Origin { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class DeleteUserTransactionBatchDTO
    {
        public int UserId { get; set; }
        public int BankAccountId { get; set; }
        public DateTime Initial { get; set; }
        public DateTime Final { get; set; }
    }

    public class GetUserTransactionsByMonthDTO
    {
        public int UserId { get; set; }
        public int BankAccountId { get; set; }
        public short Month { get; set; }
        public short Year { get; set; }
    }

    public class GetTransactionDatesDTO
    {
        public int UserId { get; set; }
        public int BankaccountId { get; set; }
    }
}
