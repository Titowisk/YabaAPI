using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using Yaba.Tools.Validations;
using YabaAPI.Abstracts;

namespace YabaAPI.Models
{
	public class BankCode : Enumeration
	{
		public static readonly BankCode BRADESCO = new BankCode(237, "Bradesco");

		private BankCode(short value, string name)
			: base(value, name)
		{
		}

		public static void ValidateCode(short value)
        {
			var valueExists = BankCode.GetAll<BankCode>().Any(item => item.Value == value);

			Validate.IsTrue(valueExists, $"Bank, with code {value}, not registered yet");
        }
	}

	// https://contasimples.com/blog/lista-de-codigos-dos-bancos/
}
