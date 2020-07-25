﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Domain.Models.Transactions;
using Yaba.Domain.Models.Users;
using Yaba.Infrastructure.DTO;
using Yaba.Tools.Validations;

namespace Yaba.Application.BankAccountServices.Impl
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankAccountService(
            IUserRepository userRepository,
            ITransactionRepository transactionRepository,
            IBankAccountRepository bankAccountRepository)
        {
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _bankAccountRepository = bankAccountRepository;
        }
        public async Task CreateBankAccountForUser(CreateUserBankAccountDTO dto)
        {
            var user = await _userRepository.GetById(dto.UserId);
            Validate.NotNull(user, "É necessário um usuário para criar uma conta de banco.");

            Validate.NotNullOrEmpty(dto.Agency, "É necessário fornecer uma agência.");

            Validate.NotNullOrEmpty(dto.Number, "É necessário fornecer o número da conta bancária.");

            BankCode.ValidateCode(dto.Code);

            var code = BankCode.FromValue<BankCode>(dto.Code);

            var bankAccount = new BankAccount(dto.Number, dto.Agency, code, dto.UserId);

            await _bankAccountRepository.Create(bankAccount);
        }

        public async Task UpdateBankAccount(UpdateUserBankAccountDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetById(dto.BankAccountId);

            Validate.NotNull(bankAccount, "Bank account not found");
            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Acesso negado");

            BankCode.ValidateCode(dto.Code);
            Validate.NotNullOrEmpty(dto.Agency, "É necessário fornecer uma agência.");
            Validate.NotNullOrEmpty(dto.Number, "É necessário fornecer o número da conta bancária.");

            bankAccount.SetAgency(dto.Agency);
            bankAccount.SetNumber(dto.Number);
            bankAccount.SetCode(dto.Code);

            await _bankAccountRepository.Update(bankAccount);
        }

        public async Task DeleteBankAccount(DeleteUserBankAccountDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetById(dto.BankAccountId);

            Validate.NotNull(bankAccount, "Bank account not found");
            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Acesso negado");

            // TODO: check if bankAccount have Transactions
            //var transactions = await _transactionRepository.GetByBankAccount(dto.BankAccountId);

            // TODO: UnitOfWork
            //if(transactions.Count() > 0)
            //{
            //    await _transactionRepository.DeleteRange(transactions);
            //}

            await _bankAccountRepository.Delete(bankAccount.Id);
        }

        public async Task<BankAccount> GetBankAccountBy(GetUserBankAccountDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetBy(dto.Agency, dto.Number, dto.Code);
            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Acesso negado");

            return bankAccount;
        }

        public async Task<BankAccountResponseDTO> GetBankAccountById(GetUserBankAccountDTO dto)
        {
            var bankAccount = await _bankAccountRepository.GetById(dto.BankAccountId);

            Validate.NotNull(bankAccount, "Bank account not found");
            Validate.IsTrue(bankAccount.UserId == dto.UserId, "Acesso negado");

            var response = new BankAccountResponseDTO()
            {
                Id = bankAccount.Id,
                Agency = bankAccount.Agency,
                AccountNumber = bankAccount.Number,
                BankCode = bankAccount.Code,
                BankName = BankCode.FromValue<BankCode>(bankAccount.Code).Name
            };

            return response;
        }

        public async ValueTask DisposeAsync()
        {
            await _bankAccountRepository.DisposeAsync();
            await _userRepository.DisposeAsync();
        }

        /*NOTES
         * Inject Token User in service or expect an UserId from a DTO?
         * Maybe is better to receive UserId in a DTO and in the Presentation layer I can
         * use this service for the logged user or for some admin user with privilege access.
         */
    }
}
