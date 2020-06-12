using System.Collections.Generic;

namespace YabaAPI.Models
{
	public class BankAccount
	{
		public BankAccount(string number, string agency)
		{
			Number = number;
			Agency = agency;
		}

		public int Id { get; set; }
		public string Number { get; set; }
		public string Agency { get; set; }
		public IEnumerable<Transaction>? Transactions { get; set; }
	}
}
