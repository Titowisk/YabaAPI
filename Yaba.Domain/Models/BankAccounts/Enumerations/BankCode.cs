using System.Linq;
using Yaba.Domain.Models.Abstracts;
using Yaba.Tools.Validations;

namespace Yaba.Domain.Models.BankAccounts.Enumerations
{
    public class BankCode : Enumeration
    {
        public static readonly BankCode BRADESCO = new BankCode(237, "Bradesco");
        public static readonly BankCode GENERICBANK = new BankCode(0, "Generic Bank");

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
}
