using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.BankStatementReaders
{
    public class BradescoReader : IBankEstatementReader
    {
        private readonly ILogger _logger;

        public BradescoReader(ILogger<BradescoReader> logger)
        {
            _logger = logger;
        }

        // TODO: Create AbstractReader.cs that implements IBankEstatementReader and make each reader to implement its own GetStandardBankStatementDTO
        public StandardBankStatementDTO ProcessBankInformation(IFormFile csvFile)
        {
            var bankStatement = new StandardBankStatementDTO();
            var config = new CsvConfiguration(new CultureInfo("pt-BR"))
            {
                Delimiter = ";",
                IgnoreBlankLines = true,
            };
            using var reader = new StreamReader(csvFile.OpenReadStream());
            using (var csvReader = new CsvReader(reader, config))
            {
                //csvReader.Configuration.Delimiter = ";";
                //csvReader.Configuration.IgnoreBlankLines = false;

                while (csvReader.Read())
                {
                    var rowIndex = csvReader.CurrentIndex;
                    if (rowIndex > 3)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(csvReader.Parser.RawRecord))
                                break;

                            //if (string.IsNullOrEmpty(csvReader.Context.RawRecord))
                            //    break;

                            if (int.TryParse(csvReader.GetField(2), out int i))
                            {
                                var transaction = CreateTransaction(csvReader);
                                bankStatement.Transactions.Add(transaction);
                            }
                            else if (!string.IsNullOrEmpty(csvReader.GetField(1)) && csvReader.Context.Parser.Record.Length == 4)
                            {
                                bankStatement.Transactions.Last().Origin = csvReader.GetField(1);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, $"{csvFile.FileName}, line: {rowIndex}");
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
            var data = csvReader.GetField(0);
            var historico = csvReader.GetField(1);
            var docto = csvReader.GetField(2);
            var credito = csvReader.GetField(3);
            var debito = csvReader.GetField(4);
            var saldo = csvReader.GetField(5);

            var transaction = new StandardTransactionDTO
            {
                Date = DateTime.ParseExact(data, new string[] { "dd/MM/yyyy", "dd/MM/yy" } , CultureInfo.GetCultureInfo("pt-BR").NumberFormat),
                TypeDescription = historico,
                TransactionUniqueHash = $"BRADESCO{docto}"
            };

            var amount = string.IsNullOrEmpty(credito) ? debito : credito;
            transaction.Amount = decimal.Parse(amount, CultureInfo.GetCultureInfo("pt-BR").NumberFormat);

            return transaction;
        }
        #endregion
    }
}
