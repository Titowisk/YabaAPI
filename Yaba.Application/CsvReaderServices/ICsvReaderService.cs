using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.CsvReaderServices
{
    public interface ICsvReaderService : IDisposable
    {
        Task<IEnumerable<FileStatusDTO>> ReadTransactionsFromFiles(CsvFileReaderDTO dto);
    }
}
