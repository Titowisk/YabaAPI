using CsvHelper;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.IO;
using Yaba.Infrastructure.DTO;

namespace Yaba.Application.BankStatementReaders
{
    // TODO: this is actually a CardStatement reader, not the bankaccount :D
    public class NuBankReader : IBankEstatementReader
    {
        public StandardBankStatementDTO ProcessBankInformation(IFormFile csvFile)
        {
            var bankStatement = new StandardBankStatementDTO();

            using var reader = new StreamReader(csvFile.OpenReadStream());
            using (var csvReader = new CsvReader(reader, new CultureInfo("pt-BR")))
            {
                csvReader.Configuration.Delimiter = ",";

                while (csvReader.Read())
                {
                    var rowIndex = csvReader.Context.Row;
                    if (rowIndex == 1) continue;

                    var transaction = CreateTransaction(csvReader);
                    bankStatement.Transactions.Add(transaction);
                }
            }

            return bankStatement;
        }

        private StandardTransactionDTO CreateTransaction(CsvReader csvReader)
        {
            var nubankRow = new NuBankRow(
                                    csvReader.GetField(0),
                                    csvReader.GetField(1),
                                    csvReader.GetField(2),
                                    csvReader.GetField(3));

            return new StandardTransactionDTO
            {
                Date = ParseDateField(nubankRow.Date),
                Amount = ParseDecimalField(nubankRow.Amount),
                Origin = nubankRow.Title,
                TransactionUniqueHash = $"NUBANK_{nubankRow.Date}_{nubankRow.Title}_{nubankRow.Amount}"
            };
        }

        private DateTime ParseDateField(string date)
        {
            return DateTime.ParseExact(date, new string[] { "yyyy-MM-dd" }, CultureInfo.GetCultureInfo("pt-BR").NumberFormat);
        }

        private decimal ParseDecimalField(string amount)
        {
            return decimal.Parse(amount, CultureInfo.GetCultureInfo("pt-BR").NumberFormat) * -1;
        }
    }

    internal class NuBankRow
    {
        public NuBankRow(string date, string category, string title, string amount)
        {
            this.Date = date;
            this.Category = category;
            this.Title = title;
            this.Amount = amount;
        }

        public string Date { get; }
        public string Category { get; }
        public string Title { get; }
        public string Amount { get; }
    }
}
