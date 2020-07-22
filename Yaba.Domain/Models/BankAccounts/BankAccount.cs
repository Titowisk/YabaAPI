using System.Collections.Generic;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Users;

namespace Yaba.Domain.Models.BankAccounts
{
	public class BankAccount
	{
		public BankAccount(string number, string agency, BankCode code, int userId)
		{
			Number = number;
			Agency = agency;
			Code = code.Value;
			UserId = userId;
			Transactions = new List<Transaction>();
		}

		public BankAccount()
		{
			// System.NotSupportedException: Deserialization of reference types without parameterless constructor is not supported. Type 'YabaAPI.
		}

		public int Id { get; private set; }
		public string Number { get; private set; }
		public string Agency { get; private set; }
		public short Code { get; private set; }
		public List<Transaction> Transactions { get; private set; }
        public User? User { get; set; }
        public int? UserId { get; set; }
    }
}
