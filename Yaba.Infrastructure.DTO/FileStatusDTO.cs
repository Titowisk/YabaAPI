using System;

namespace Yaba.Infrastructure.DTO
{
    public class FileStatusDTO
    {
        public string FileName { get; set; }
        public bool IsSuccessfullRead { get; set; }
        public int TransactionsSaved { get; set; }
        public int TransactionsRead { get; set; }
        public int ExistentTransactionsSkiped { get { return TransactionsRead - TransactionsSaved; }  }
        public DateTime? InitialDate { get; set; }
        public DateTime? FinalDate { get; set; }
    }
}
