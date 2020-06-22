using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YabaAPI.Abstracts;
using YabaAPI.Models;
using YabaAPI.Repositories;

namespace YabaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CsvReaderController : ControllerBase
	{
		private readonly DataContext _context;

		public CsvReaderController(DataContext context)
        {
			_context = context;
        }

		[Route("[Action]")]
		[HttpPost]
		public async Task<ActionResult<string>> ReadCsvFileFromForm(IFormFileCollection csvFiles, [FromForm] short bankCode)
		{
			// TODO: validate csv file

			// TODO: validate bankCode exists in BankCode Enumeration Class

			// TODO: dinamically choose which BankReader to use for reading

			foreach (var csv in csvFiles)
			{
				var parsedFile = BradescoReader(csv);

				SaveTransactions(parsedFile, bankCode);
			}
			
			// TODO: custom error handling for each batch of transactions saved

			// TODO: return number of files processed and status for each one
			return "olá";
		}



        #region Priv Methods
        private void SaveTransactions(BradescoCsvFile parsedFile, short bankCode)
        {
			// TODO: regex to get Agency and Account Numbers
			var splitFirstLine = parsedFile.FirstLine.Split("|");
			var agencyNumber = splitFirstLine[0].Split(":")[2].Trim();
			var accountNumber = splitFirstLine[1].Split(":")[1].Trim().Replace("-", "");
			var enumBankCode = BankCode.FromValue<BankCode>(bankCode);

            

			var newBankAccount = new BankAccount(accountNumber, agencyNumber, enumBankCode);

			var bankAccount = _context.BankAccounts.FirstOrDefault(bk => bk.Equals(newBankAccount));
			if (bankAccount == null)
            {
				bankAccount = newBankAccount;
			}

			foreach (var data in parsedFile.BradescoTransactions)
			{
                try
                {
					var amount = string.IsNullOrEmpty(data.Credito) ? data.Debito : data.Credito;

					var newTransaction = new Transaction(
						data.Origem,
						DateTime.Parse(data.Data),
						decimal.Parse(amount));
				
					// TODO: check if docto number exists in DB
					_context.Transactions.Add(newTransaction);

					bankAccount.Transactions.Add(newTransaction);
                }
                catch (Exception ex)
                {

                }

			}

			_context.SaveChanges();
			// TODO: change to async Task
		}

        private BradescoCsvFile BradescoReader(IFormFile csv)
		{
			// TODO: change to async Task
			var bradescoCsv = new BradescoCsvFile();

			using var reader = new StreamReader(csv.OpenReadStream());
			using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				csvReader.Configuration.Delimiter = ";";
				csvReader.Configuration.MissingFieldFound = null; // TODO: this removes the need to use the try/catch block (test)

				while (csvReader.Read())
				{
					var rowIndex = csvReader.Context.Row;
					if(rowIndex > 3)
                    {
                        try
                        {
                            if (int.TryParse(csvReader.GetField(2), out int i))
                            {
                                var transaction = new BradescoTransaction();
                                transaction.Data = csvReader.GetField(0);
                                transaction.Historico = csvReader.GetField(1);
                                transaction.Docto = csvReader.GetField(2);
                                transaction.Credito = csvReader.GetField(3);
                                transaction.Debito = csvReader.GetField(4);
                                transaction.Saldo = csvReader.GetField(5);

                                bradescoCsv.BradescoTransactions.Add(transaction);
                            }
                            else if (!string.IsNullOrEmpty(csvReader.GetField(1)) && csvReader.Context.Record.Length == 4)
                            {
                                bradescoCsv.BradescoTransactions.Last().Origem = csvReader.GetField(1);
                            }

                        }
                        catch (Exception ex)
                        {
							    
                        }

					}
					else if (rowIndex == 1)
					{
						bradescoCsv.FirstLine = csvReader.GetField(0);
					}
					
				}
			};

			return bradescoCsv;
		}

		#endregion
	}

	public class BradescoCsvFile
	{
        public BradescoCsvFile()
        {
			BradescoTransactions = new List<BradescoTransaction>();
		}

		public string FirstLine { get; set; } // Extrato de: Ag: 1425 | Conta: 711093-6
		public string Header { get; set; } // Data;Hist�rico;Docto.;Cr�dito (R$);D�bito (R$);Saldo (R$);

		public List<BradescoTransaction> BradescoTransactions { get; }
	}

	public class BradescoTransaction
	{
		public string Data { get; set; }
		public string Historico { get; set; }
		public string Docto { get; set; }
		public string Credito { get; set; }
		public string Debito { get; set; }
		public string Saldo { get; set; }
        public string Origem { get; set; }

    }

	/*NOTES
	 * Class is to big. There is a clear need of breaking down in more classes
	 */
}
