using System;

namespace YabaAPI.Models
{
	public class Transaction
	{
		public long Id { get; private set; }
		public string? Origin { get; private set; }
		public DateTime Date { get; private set; }
		public decimal Amount { get; private set; }
	}
}
