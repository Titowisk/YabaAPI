﻿using System;

namespace Yaba.Infrastructure.DTO
{
    public class TransactionDTO
    {
        public long Id { get; set; }
        public string Origin { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int BankAccountId { get; set; }
    }
}
