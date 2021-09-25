using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using Xunit;
using Yaba.Application.BankStatementReaders;

namespace Yaba.Tests.Application.BankStatementReaders
{
    public class BradescoReaderServiceTest
    {
        [Fact]
        public void ProcessBankInformation_CreateTransactionsFromACsvFile()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite("BradescoReaderServiceTest");

            // Given one csv file downloaded from Bradesco
            //  ant the file has 10 transactions
            var csvFile = GetCsvFile("BradescoStatement_10Transactions");
            Mock<IFormFile> fileMock = MockFile(csvFile);

            // when I call ProcessBankInformation from the read file
            var readerService = (IBankEstatementReader)serviceProvider.GetService(typeof(BradescoReader));
            var resultStatement = readerService.ProcessBankInformation(fileMock.Object);

            // then it should create 10 transactions for the Bradesco BankAccount
            Assert.Equal(10, resultStatement.Transactions.Count);

        }

        #region Private Methods

        // TODO: Repeated code: MockFile - Refactor?
        private Mock<IFormFile> MockFile(FileInfo csvFile)
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.FileName).Returns(csvFile.Name);
            fileMock.Setup(_ => _.Length).Returns(csvFile.Length);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(csvFile.OpenRead());
            return fileMock;
        }

        private FileInfo GetCsvFile(string fileName)
        {
            var path = Directory.GetCurrentDirectory();
            var projectPath = Directory.GetParent(path).Parent.Parent.FullName;

            return new FileInfo($"{projectPath}\\CsvFiles\\BradescoReaderServiceTest\\{fileName}.csv");
        }
        #endregion
    }
}
