using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.CsvReaderServices
{
    public interface ICsvReaderService
    {
        Task<IEnumerable<FileStatusDTO>> ReadTransactionsFromFiles(IFormFileCollection csvFiles, short bankCode);
    }
}
