using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yaba.Application.BankStatementReaders;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.CsvReaderServices.Impl
{
    public class CsvReaderService : ICsvReaderService
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public CsvReaderService(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public Task<IEnumerable<FileStatusDTO>> ReadTransactionsFromFiles(IFormFileCollection csvFiles, short bankCode)
        {
            IBankEstatementReader reader = BankStatementReaderFactory.GetReader(bankCode);

            foreach (var csv in csvFiles)
            {
                try
                {
                    // TODO: validate file

                    var parsedFile = reader.ProcessBankInformation(csv);

                    PersistBankEstatementInformation(parsedFile);
                }
                catch (Exception)
                {
                    // TODO: logger
                }
            }
        }
    }
}
