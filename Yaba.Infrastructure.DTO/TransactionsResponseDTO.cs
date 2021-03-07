using System;
using System.Collections.Generic;

namespace Yaba.Infrastructure.DTO
{
    public class TransactionsDateFilterResponseDTO
    {
        public long Id { get; set; }
        public string Origin { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Category { get; set; }
        public short? CategoryId { get; set; }
    }

    public class ExistentTransactionsDatesResponseDTO
    {
        public int Year { get; set; }
        public HashSet<int> Months { get; set; } = new HashSet<int>();
    }

    public class CategoryDTO
    {
        public int Key { get; set; }
        public int Value { get; set; }
        public string Text { get; set; }
    }
}
