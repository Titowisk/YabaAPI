using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Yaba.Application.BankStatementReaders;

namespace Yaba.Tests.Application.BankStatementReaders
{
    public class NuBankReaderServiceTest
    {
        [Fact]
        public void ProcessBankInformation_CreateTransactionsFromACsvFile()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite("NuBankReaderServiceTest");

            // given one csv file download from NuBank
            //  and the file has 10 transactions

            var csvFile = GetCsvFile("ProcessBankInformation_1");
            Mock<IFormFile> fileMock = MockFile(csvFile);

            // when I call ProcessBankInformation from the read file
            var readerService = (IBankEstatementReader)serviceProvider.GetService(typeof(NuBankReader));
            var resultStatement = readerService.ProcessBankInformation(fileMock.Object);

            // then it should create 10 transactions for the NuBank BankAccount
            Assert.Equal(10, resultStatement.Transactions.Count);
        }

        [Fact(DisplayName = "ProcessBankInformation should deal with problematic fields")]
        public void ProcessBankInformation_BadDataRow()
        {
            var serviceProvider = DependencyInversion.DependencyContainer.GetServicesUsingSQLite("NuBankReaderServiceTest");

            // given one csv file downloaded from NuBank with problematic fields
            //  that has 2 data rows:
            //      first data row has a field with one quote
            //      second data row has a field with double quotes
            var csvFile = GetCsvFile("NubankCardStatement_ProblematicFields");
            Mock<IFormFile> fileMock = MockFile(csvFile);

            // when I call ProcessBankInformation from the read file
            var readerService = (IBankEstatementReader)serviceProvider.GetService(typeof(NuBankReader));
            var resultStatement = readerService.ProcessBankInformation(fileMock.Object);

            // then it should handle and read all data rows
            Assert.Equal(2, resultStatement.Transactions.Count);

            var firstDataRow = resultStatement.Transactions[0];
            Assert.Equal("IOF de Paypal *Italki Hk", firstDataRow.Origin);
            Assert.Equal(-2.37M, firstDataRow.Amount);

            var secondDataRow = resultStatement.Transactions[1];
            Assert.Equal("Italki Hk Limited", secondDataRow.Origin);
            Assert.Equal(-171.26M, secondDataRow.Amount);
        }

        #region Priv Methods
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

            return new FileInfo($"{projectPath}\\CsvFiles\\NuBankReaderServiceTest\\{fileName}.csv");
        }
        //private static IFormFile AsMockFromFileInfo(this FileInfo readFileFromPath)
        //{
        //    var mockIFormFile = new Mock<IFormFile>();
        //    mockIFormFile.Setup(iff => iff.ContentType).Returns("text/csv");
        //}
        #endregion
    }
}
