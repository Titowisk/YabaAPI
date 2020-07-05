using CsvHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.BankStatementReaders
{
    public class BradescoReader : IBankEstatementReader
    {
        public StandardBankStatementDTO ProcessBankInformation(IFormFile csvFile)
        {
            var bankStatement = new StandardBankStatementDTO();

            using var reader = new StreamReader(csvFile.OpenReadStream());
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csvReader.Configuration.Delimiter = ";";
                csvReader.Configuration.MissingFieldFound = null; // TODO: this removes the need to use the try/catch block (test)
                csvReader.Configuration.IgnoreBlankLines = false;

                while (csvReader.Read())
                {
                    var rowIndex = csvReader.Context.Row;
                    if (rowIndex > 3)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(csvReader.Context.RawRecord))
                                break;

                            if (int.TryParse(csvReader.GetField(2), out int i))
                            {
                                var transaction = CreateTransaction(csvReader);
                                bankStatement.Transactions.Add(transaction);
                            }
                            else if (!string.IsNullOrEmpty(csvReader.GetField(1)) && csvReader.Context.Record.Length == 4)
                            {
                                bankStatement.Transactions.Last().Origin = csvReader.GetField(1);
                            }
                        }
                        catch (Exception ex)
                        {
                            // TODO: add logger
                        }
                    }
                    else if (rowIndex == 1)
                    {
                        // TODO: encapsulate this block of code ?
                        var accountAndAgencyNumbers = csvReader.GetField(0);
                        var splitFirstLine = accountAndAgencyNumbers.Split("|");
                        bankStatement.AgencyNumber = splitFirstLine[0].Split(":")[2].Trim();
                        bankStatement.AccountNumber = splitFirstLine[1].Split(":")[1].Trim().Replace("-", "");
                    }

                }
            };

            return bankStatement;
        }

        #region Priv Methods
        private StandardTransactionDTO CreateTransaction(CsvReader csvReader)
        {
            // variable names according to csv bradesco header
            var data = csvReader.GetField(0);
            var historico = csvReader.GetField(1);
            var docto = csvReader.GetField(2);
            var credito = csvReader.GetField(3);
            var debito = csvReader.GetField(4);
            var saldo = csvReader.GetField(5);

            var transaction = new StandardTransactionDTO();
            transaction.Date = DateTime.Parse(data);
            transaction.TypeDescription = historico;

            var amount = string.IsNullOrEmpty(credito) ? debito : credito;
            transaction.Amount = decimal.Parse(amount);

            return transaction;
        }
        #endregion
    }
}
