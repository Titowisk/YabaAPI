using System;

namespace Yaba.Infrastructure.DTO
{
    public class TransactionsDateFilterResponseDTO
    {
        public string Origin { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
