using System;

namespace YabaAPI.Models
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

		public long Id { get; set; }
		public string? Origin { get; set; }
		public DateTime Date { get; set; }
		public decimal Amount { get; set; }
		public int? BankAccountId { get; set; }
		public BankAccount? BankAccount { get; set; }
	}
}
