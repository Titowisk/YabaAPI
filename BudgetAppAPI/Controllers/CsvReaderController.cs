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

namespace YabaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CsvReaderController : ControllerBase
	{

		//[Route("[Action]")]
		//[HttpPost]
		//public async Task<IActionResult> ReadCsvFileFromBinary(byte[] csvBinary)
		//{
		//	// TODO: validate csv file
		//	using var stream = System.IO.File.ReadAllBytesAsync(csvBinary);

		//}

		[Route("[Action]")]
		[HttpPost]
		public async Task<ActionResult<string>> ReadCsvFileFromForm(IFormFileCollection csvFiles, [FromForm] short bankCode)
		{
			// TODO: validate csv file

			// TODO: validate bankCode exists in BankCode Enumeration Class

			// TODO: dinamically choose which BankReader to use for reading

			foreach (var csv in csvFiles)
			{
				BradescoReader(csv);
			}
			
			return "olá";
		}


		#region Priv Methods
		private BradescoCsvFile BradescoReader(IFormFile csv)
		{
			var bradescoCsv = new BradescoCsvFile();

			using var reader = new StreamReader(csv.OpenReadStream());
			using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				csvReader.Configuration.Delimiter = ";";
				csvReader.Configuration.MissingFieldFound = null;

				while (csvReader.Read())
				{
					var rowIndex = csvReader.Context.Row;
					if(rowIndex > 3)
                    {
                        try
                        {
							if(!string.IsNullOrEmpty(csvReader.GetField(2)))
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
							else if (!string.IsNullOrEmpty(csvReader.GetField(1)) && csvReader.GetField(1) != "Total do Dia")
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
						// TODO: regex to get Agency and Account Numbers
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
}
