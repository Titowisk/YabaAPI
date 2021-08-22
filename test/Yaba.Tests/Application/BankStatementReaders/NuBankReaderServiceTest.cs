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
