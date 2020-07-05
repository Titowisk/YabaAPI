﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yaba.Application.BankStatementReaders;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Infrastructure.DTO;
using Yaba.Tools.Validations;

namespace Yaba.Application.CsvReaderServices.Impl
{
    public class CsvReaderService : ICsvReaderService
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public CsvReaderService(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<IEnumerable<FileStatusDTO>> ReadTransactionsFromFiles(IFormFileCollection csvFiles, short bankCode)
        {
            IBankEstatementReader reader = BankStatementReaderFactory.GetReader(bankCode);
            var fileStatusResult = new List<FileStatusDTO>();

            foreach (var csv in csvFiles)
            {
                var fileStatus = new FileStatusDTO();

                try
                {
                    Validate.IsTrue(csv.ContentType == "text/csv", "Only csv files are accepted");

                    Validate.IsTrue(csv.Length > 0, $"File {csv.FileName} is empty");

                    var parsedFile = reader.ProcessBankInformation(csv);

                    await PersistBankEstatementInformation(parsedFile, bankCode);

                    fileStatus.IsSuccessfullRead = true;
                }
                catch (Exception)
                {
                    // TODO: logger
                    fileStatus.IsSuccessfullRead = false;
                }
                finally
                {
                    fileStatusResult.Add(fileStatus);
                }
            }

            return fileStatusResult;
        }

        private async Task PersistBankEstatementInformation(StandardBankStatementDTO parsedFile, short bankCode)
        {
            var enumBankCode = BankCode.FromValue<BankCode>(bankCode);

            var newBankAccount = new BankAccount(parsedFile.AccountNumber, parsedFile.AgencyNumber, enumBankCode);

            var bankAccount = _bankAccountRepository.Find(newBankAccount);
            if (bankAccount == null)
            {
                bankAccount = newBankAccount;
            }

            foreach (var data in parsedFile.Transactions)
            {
                try
                {
                    // TODO: create mapping between Transaction and StandardTransaction
                    var newTransaction = new Transaction(
                        data.Origin,
                        data.Date,
                        data.Amount);

                    bankAccount.Transactions.Add(newTransaction);
                }
                catch (Exception)
                {
                    // TODO: add logger ?
                }
            }

            if (bankAccount.Id > 0)
                await _bankAccountRepository.Update(bankAccount);
            else
                await _bankAccountRepository.Create(bankAccount);
        }
    }
}
