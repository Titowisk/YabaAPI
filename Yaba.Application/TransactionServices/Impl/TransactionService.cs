using System.Collections.Generic;
using System.Threading.Tasks;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.Transactions;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Persistence.UnitOfWork;
using Yaba.Tools.Validations;
using Transaction = Yaba.Domain.Models.Transactions.Transaction;

namespace Yaba.Application.TransactionServices.Impl
{
    public class TransactionService : ITransactionService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly UnitOfWork _uow;

        public TransactionService(
            UnitOfWork uow,
            ITransactionRepository transactionRepository,
            IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _transactionRepository = transactionRepository;
            _uow = uow;
        }

        public async Task Create(CreateUserTransactionDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetById(dto.BankAccountId);
            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Acesso negado");

            // TODO: Validations?
            var transaction = new Transaction(dto.Origin, dto.Date, dto.Amount);

            _transactionRepository.Delete(transaction);

            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na remoção da transação");
        }

        public async Task Delete(DeleteUserTransactionDTO dto)
        {
            var transaction = await _transactionRepository.GetById(dto.TransactionId);
            Validate.NotNull(transaction, "Transação não encontrada");
            Validate.IsTrue(transaction.BankAccountId == dto.BankAccountId, "Acesso negado");

            var bankAccount = await _bankAccountRepository.GetById(transaction.BankAccountId);
            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Acesso negado");

            _transactionRepository.Delete(transaction);

            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na remoção da transação");
        }

        public async Task DeleteBatchBetween(DeleteUserTransactionBatchDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetById(dto.BankAccountId);
            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Acesso negado");

            var transactions = await _transactionRepository.GetByDate(dto.Initial, dto.Final, dto.BankAccountId);

            _transactionRepository.DeleteRange(transactions);

            Validate.IsTrue(await _uow.CommitAsync(), "Ocorreu um problema na remoção das transações");
        }

        public async Task<IEnumerable<Transaction>> GetByMonth(GetUserTransactionsByMonthDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetById(dto.BankAccountId);
            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Acesso negado");

            var transactions = await _transactionRepository.GetByMonth(dto.BankAccountId, dto.Month, dto.Year);
            Validate.IsTrue(transactions.Count() > 0, $"Não foram encontradas transações para a data {dto.Month}/{dto.Year}");

            return transactions;
        }

        public void Dispose()
        {
            _uow.Dispose(); // TODO: does it really dispose all resources from the context (BankAccount, User, Transaction ) ??
        }
    }
}
