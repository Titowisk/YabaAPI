using System.Collections.Generic;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Users;

namespace Yaba.Domain.Models.BankAccounts
{
	public class BankAccount
	{
		public BankAccount(string number, string agency, BankCode code)
		{
			Number = number;
			Agency = agency;
			Code = code.Value;
		}

		public BankAccount()
		{
			// System.NotSupportedException: Deserialization of reference types without parameterless constructor is not supported. Type 'YabaAPI.
		}

		public int Id { get; set; }
		public string Number { get; set; }
		public string Agency { get; set; }
		public short Code { get; set; }
		public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public User? User { get; set; }
        public int? UserId { get; set; }
    }
}
