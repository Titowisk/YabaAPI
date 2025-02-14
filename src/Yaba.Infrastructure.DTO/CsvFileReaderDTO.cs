using Microsoft.AspNetCore.Http;

namespace Yaba.Infrastructure.DTO
{
    public class CsvFileReaderDTO
    {
        public int FilesOwnerId { get; set; }
        public int BankAccountId { get; set; }
        public IFormFileCollection CsvFiles { get; set; }
    }
}
