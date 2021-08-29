using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yaba.Application.CsvReaderServices;
using Yaba.Domain.Models.BankAccounts;
using Yaba.Domain.Models.BankAccounts.Enumerations;
using Yaba.Infrastructure.DTO;
using Yaba.Infrastructure.Persistence.Context;
using Yaba.Tests.EntitiesCreator.BankAccountEntity;

namespace Yaba.Tests.Application
{
    public class CsvReaderServiceTest
    {
        [Fact]
        public async Task ReadTransactionsFromFiles_NubankCsvFile()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite("CsvReaderServiceTest");
            // given a bankAccount
            //  and a card statement from nubank with 10 expenses
            var response = BankAccountBuilder
                .CreateABankAccount(serviceProvider)
                .WithBankCode(BankCode.NUBANK)
                .Build();

            var csvFile = GetCsvFile("ProcessBankInformation_1"); // TODO: Duplicated Code -> refactor
            Mock<IFormFile> fileMock = MockFile(csvFile);

            var fileCollectionMock = new Mock<IFormFileCollection>();
            var fileList = new List<IFormFile>() { fileMock.Object };
            fileCollectionMock.Setup(_ => _.GetEnumerator()).Returns(fileList.GetEnumerator());

            // when I use service ReadTransactionsFromFiles
            var csvReaderService = (ICsvReaderService)serviceProvider.GetService(typeof(ICsvReaderService));
            var dto = new CsvFileReaderDTO
            {
                BankAccountId = response.BankAccount.Id,
                FilesOwnerId = response.User.Id,
                CsvFiles = fileCollectionMock.Object
            };
            var result = await csvReaderService.ReadTransactionsFromFiles(dto);

            // then the result of reading should be successul
            Assert.Single(result);
            var fileStatus = result.First();
            Assert.True(fileStatus.IsSuccessfullRead);
            Assert.Equal(10, fileStatus.TransactionsRead);
            Assert.Equal(10, fileStatus.TransactionsSaved);
            Assert.Equal(0, fileStatus.ExistentTransactionsSkiped);

            //  and those 10 transactions should be saved for the given bankAccount
            var context = (DataContext)serviceProvider.GetService(typeof(DataContext));
            var bankAccount = context.BankAccounts.Include(bk => bk.Transactions).First(bk => bk.Id == response.BankAccount.Id);
            Assert.Equal(10, bankAccount.Transactions.Count);
        }

        #region Priv Methods
        private Mock<IFormFile> MockFile(FileInfo csvFile)
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.ContentType).Returns("text/csv");
            fileMock.Setup(_ => _.FileName).Returns(csvFile.Name);
            fileMock.Setup(_ => _.Length).Returns(csvFile.Length);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(csvFile.OpenRead());
            return fileMock;
        }

        private FileInfo GetCsvFile(string fileName)
        {
            var path = Directory.GetCurrentDirectory();
            var projectPath = Directory.GetParent(path).Parent.Parent.FullName;

            return new FileInfo($"{projectPath}\\CsvFiles\\NuBankReaderServiceTest\\{fileName}.csv");
        }
        #endregion
    }
}
