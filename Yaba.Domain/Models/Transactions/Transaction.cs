using System;
using Yaba.Domain.Models.BankAccounts;

namespace Yaba.Domain.Models.Transactions
{
	public class Transaction
	{
		public Transaction(string origin, DateTime date, decimal amount)
		{
			Origin = origin;
			Date = date;
			Amount = amount;
		}

		public Transaction() 
		{
			// System.NotSupportedException: Deserialization of reference types without parameterless constructor is not supported. Type 'YabaAPI.Models.Transaction'
		}

		public long Id { get; private set; }
		public string? Origin { get; private set; }
		public DateTime Date { get; private set; }
		public decimal Amount { get; private set; }
		public int? BankAccountId { get; private set; }
		public BankAccount? BankAccount { get; set; }
		public Category? Category { get; set; }
	}
}
