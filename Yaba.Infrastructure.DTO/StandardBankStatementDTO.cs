using System;
using System.Collections.Generic;

namespace Yaba.Infrastructure.DTO
{
    public class StandardBankStatementDTO
    {
        public StandardBankStatementDTO()
        {
            Transactions = new List<StandardTransactionDTO>();
        }
        public string AgencyNumber { get; set; }
        public string AccountNumber { get; set; }

        public List<StandardTransactionDTO> Transactions { get; private set; }
    }

    public class StandardTransactionDTO
    {
        public string Origin { get; set; }
        public string TypeDescription { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

}
