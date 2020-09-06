using System;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.Transactions.Enumerations;

namespace Yaba.Domain.Models.Transactions
{
	public class Transaction
	{
		public Transaction(string origin, DateTime date, decimal amount, int bankAccountId)
		{
			Origin = origin;
			Date = date;
			Amount = amount;
			BankAccountId = bankAccountId;
		}

		public Transaction() 
		{
			// System.NotSupportedException: Deserialization of reference types without parameterless constructor is not supported. Type 'YabaAPI.Models.Transaction'
		}

		public long Id { get; private set; }
		public string? Origin { get; private set; }
		public DateTime Date { get; private set; }
		public decimal Amount { get; private set; }
		public int BankAccountId { get; set; }
		public BankAccount BankAccount { get; set; }
		public Category? Category { get; set; }
        public string? Metadata { get; set; }

        public void SetOrigin(string? origin)
        {
			if (origin == null)
				return;

			Origin = origin;
        }

		public void SetDate(DateTime date)
        {
			Date = date;
        }

		public void SetAmount(decimal amount)
        {
			Amount = amount;
        }
	}
}
