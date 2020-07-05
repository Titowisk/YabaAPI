using System;

namespace Yaba.Infrastructure.DTO
{
    public class FileStatusDTO
    {
        public string FileName { get; set; }
        public bool IsSuccessfullRead { get; set; }
        public int? TransactionsSaved { get; set; }
        public DateTime? InitialDate { get; set; }
        public DateTime? FinalDate { get; set; }
    }
}
