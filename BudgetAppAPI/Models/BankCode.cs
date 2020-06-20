using YabaAPI.Abstracts;

namespace YabaAPI.Models
{
	public class BankCode : Enumeration<short>
	{
		public static readonly BankCode BRADESCO = new BankCode(237, "Bradesco");

		private BankCode(short value, string name)
			: base(value, name)
		{
		}
	}

	// https://contasimples.com/blog/lista-de-codigos-dos-bancos/
}
