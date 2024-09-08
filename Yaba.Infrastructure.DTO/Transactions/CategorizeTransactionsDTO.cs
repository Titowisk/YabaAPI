namespace Yaba.Infrastructure.DTO.Transactions
{
    public class CategorizeTransactionsQueryDTO
    {
        public int UserId { get; set; }
        public int BankAccountId { get; set; }
        public long TransactionId { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public string Origin { get; set; }
    }

    public class CategorizeTransactionsBodyDTO
    {
        public short CategoryId { get; set; }
    }
}
