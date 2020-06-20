using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using YabaAPI.Models;
using YabaAPI.Repositories;

namespace YabaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionsController : ControllerBase
	{
		private readonly DataContext _context;

		public TransactionsController(DataContext context)
		{
			_context = context;
		}

		[HttpPost]
		public IActionResult Create([FromBody] Transaction transaction)
		{
			try
			{
				_context.Transactions.Add(transaction);
				_context.SaveChanges();

				return Ok();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		[HttpGet]
		public IActionResult Get()
		{
			try
			{
				var transactions = _context
					.Transactions
					.Include(t => t.BankAccount)
					.ToList();

				var results = from t in transactions
							 select new 
							 {
								 t.Id,
								 t.Origin,
								 t.Amount,
								 t.Date,
								 Bank = t.BankAccount != null ? t.BankAccount.Number : "",
								 Category = t.Category != null ? t.Category.ToString() : ""
							 };

				return Ok(results);
			}
			catch (Exception)
			{
				throw;
			}
		}

		[HttpGet("{id}")]
		public IActionResult Get(long id)
		{
			try
			{
				var transaction = _context
					.Transactions
					.Include(t => t.BankAccount)
					.First(t => t.Id == id);

				var result = new
				{
					transaction.Id,
					transaction.Origin,
					transaction.Amount,
					transaction.Date,
					Bank = transaction.BankAccount != null ? BankCode.FromValue<BankCode>(transaction.BankAccount.Code).Name : "",
					Category = transaction.Category != null ? transaction.Category.ToString() : ""
				};

				return Ok(result);
			}
			catch (Exception)
			{
				throw;
			}
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(long id)
		{
			try
			{
				var transaction = _context.Transactions.Find(id);
				_context.Transactions.Remove(transaction);
				_context.SaveChanges();

				return Ok();
			}
			catch (Exception)
			{
				throw;
			}
		}

		[HttpPut("{id}")]
		public IActionResult Update(long id, [FromBody] Transaction newTransaction)
		{
			try
			{
				var transaction = _context.Transactions.Find(id);
				transaction.Origin = newTransaction.Origin;
				transaction.Date = newTransaction.Date;
				transaction.Amount = newTransaction.Amount;
				transaction.BankAccountId = newTransaction.BankAccountId;

				_context.Transactions.Update(transaction);
				_context.SaveChanges();

				return Ok();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
	/* NOTES:
		- Most parsers use ISO 8601 (talking about dates: 2020-01-01T17:16:40)
		- Return types
			https://docs.microsoft.com/pt-br/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.accepted?view=aspnetcore-3.1
		- using .Include() normally, it brings the whole object in the response.

		- making fields nullable causes the necessity of checking values before returning
		- enum is less verbose than Enumeration when , but Enumeration is less verbose for validations
	 */
}